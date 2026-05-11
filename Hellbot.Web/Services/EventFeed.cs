using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Audio;
using Hellbot.Core.Events.Chat;
using Hellbot.Core.Events.Session;
using Hellbot.UI.Components.Model;
using Hellbot.UI.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace Hellbot.UI.Services;

public sealed class EventFeed : IAsyncDisposable
{
    private const int MaxChatLines = 500;
    private const int MaxHubEvents = 100;

    private readonly IOptions<HellbotApiOptions> _apiOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SemaphoreSlim _startGate = new(1, 1);

    private readonly List<ChatFeedLine> _chatLines = [];
    private readonly List<HubEventMessage> _hubEvents = [];
    private readonly List<ServiceStatusRowDto> _serviceStatusRows = [];

    private static readonly JsonSerializerOptions ChatJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions ServiceStatusJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private HubConnection? _hubConnection;
    private bool _started;
    private bool _serviceStatusSnapshotLoaded;

    public EventFeed(IOptions<HellbotApiOptions> apiOptions, IHttpClientFactory httpClientFactory)
    {
        _apiOptions = apiOptions;
        _httpClientFactory = httpClientFactory;
        ChatLines = new ReadOnlyCollection<ChatFeedLine>(_chatLines);
        HubEvents = new ReadOnlyCollection<HubEventMessage>(_hubEvents);
        ServiceStatusRows = new ReadOnlyCollection<ServiceStatusRowDto>(_serviceStatusRows);
    }

    public IReadOnlyList<ChatFeedLine> ChatLines { get; }
    public IReadOnlyList<HubEventMessage> HubEvents { get; }
    public IReadOnlyList<ServiceStatusRowDto> ServiceStatusRows { get; }

    public int ServiceStatusTableVersion { get; private set; }

    public event Action? Changed;

    public async Task EnsureStartedAsync()
    {
        await _startGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_started)
                return;

            var hubUrl = $"{_apiOptions.Value.BaseUrl.TrimEnd('/')}/eventsHub";
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<HubEventMessage>("ReceiveEvent", OnReceiveEvent);

            await _hubConnection.StartAsync().ConfigureAwait(false);
            _started = true;
        }
        finally
        {
            _startGate.Release();
        }
    }

    private void OnReceiveEvent(HubEventMessage raw)
    {
        ApplyChatEvent(raw);
        ApplyHubLogEvent(raw);
        ApplyServiceStatusEvent(raw);
        Changed?.Invoke();
    }

    private void ApplyChatEvent(HubEventMessage raw)
    {
        if (raw.Type == nameof(ChatMessageReceived))
        {
            ChatReceivedPayload? payload;
            try
            {
                payload = raw.Data.Deserialize<ChatReceivedPayload>(ChatJsonOptions);
            }
            catch
            {
                return;
            }

            if (payload is null)
                return;

            var userDisplay = raw.User is { } u ? (u.Username ?? u.UserId) : "—";
            var sourceDisplay = raw.Source.ToString();

            PrependChatLine(new ChatFeedLine(
                raw.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"),
                sourceDisplay,
                userDisplay,
                payload.Message));
            return;
        }

        if (raw.Type != nameof(VoiceTranscriptionCompleted))
            return;

        VoiceTranscriptionPayload? vtPayload;
        try
        {
            vtPayload = raw.Data.Deserialize<VoiceTranscriptionPayload>(ChatJsonOptions);
        }
        catch
        {
            return;
        }

        if (vtPayload is null)
            return;

        PrependChatLine(new ChatFeedLine(
            raw.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"),
            "Mic",
            "CursedMoose",
            vtPayload.Text));
    }

    private void PrependChatLine(ChatFeedLine line)
    {
        _chatLines.Insert(0, line);
        while (_chatLines.Count > MaxChatLines)
            _chatLines.RemoveAt(_chatLines.Count - 1);
    }

    private void ApplyHubLogEvent(HubEventMessage raw)
    {
        var evt = raw.Id == Guid.Empty ? raw with { Id = Guid.NewGuid() } : raw;
        _hubEvents.Insert(0, evt);
        if (_hubEvents.Count > MaxHubEvents)
            _hubEvents.RemoveAt(_hubEvents.Count - 1);
    }

    private void ApplyServiceStatusEvent(HubEventMessage evt)
    {
        if (evt.Type != nameof(WebsocketStateChanged)
            || !TryReadStatePayload(evt.Data, out var status, out var details))
            return;

        var ix = _serviceStatusRows.FindIndex(r => r.Platform == evt.Source.Platform);
        if (ix < 0)
            return;

        _serviceStatusRows[ix] = new ServiceStatusRowDto
        {
            Platform = evt.Source.Platform,
            Status = status,
            Details = details,
            LastChanged = evt.Timestamp
        };
        ServiceStatusTableVersion++;
    }

    public async Task EnsureServiceStatusSnapshotAsync()
    {
        if (_serviceStatusSnapshotLoaded)
            return;

        var client = _httpClientFactory.CreateClient("api");
        var list = await client.GetFromJsonAsync<List<ServiceStatusRowDto>>("service-status", ServiceStatusJsonOptions)
            .ConfigureAwait(false);
        if (list is null)
            return;

        _serviceStatusRows.Clear();
        _serviceStatusRows.AddRange(list);
        ServiceStatusTableVersion++;
        _serviceStatusSnapshotLoaded = true;
        Changed?.Invoke();
    }

    private static bool TryReadStatePayload(JsonElement data, out ConnectionState status, out string? details)
    {
        status = ConnectionState.Initialized;
        details = null;

        if (!TryGetPropertyIgnoreCase(data, "Status", out var stEl))
            return false;

        if (stEl.ValueKind == JsonValueKind.String)
        {
            if (!Enum.TryParse(stEl.GetString(), ignoreCase: true, out status))
                return false;
        }
        else if (stEl.ValueKind == JsonValueKind.Number)
            status = (ConnectionState)stEl.GetInt32();
        else
            return false;

        if (TryGetPropertyIgnoreCase(data, "Details", out var dEl) && dEl.ValueKind == JsonValueKind.String)
            details = dEl.GetString();

        return true;
    }

    private static bool TryGetPropertyIgnoreCase(JsonElement el, string name, out JsonElement prop)
    {
        foreach (var p in el.EnumerateObject())
        {
            if (string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                prop = p.Value;
                return true;
            }
        }

        prop = default;
        return false;
    }

    public async ValueTask DisposeAsync()
    {
        await _startGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_hubConnection is not null)
                await _hubConnection.DisposeAsync().ConfigureAwait(false);
            _hubConnection = null;
        }
        finally
        {
            _startGate.Release();
        }

        _startGate.Dispose();
    }
}

public sealed record ChatFeedLine(
    string TimestampDisplay,
    string Source,
    string UserDisplay,
    string Message);
