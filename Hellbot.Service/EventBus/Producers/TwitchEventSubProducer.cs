using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;
using Hellbot.Service.Clients.Twitch;
using Hellbot.Service.Config;
using Microsoft.Extensions.Options;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.EventSub;
using TwitchLib.EventSub.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;

namespace Hellbot.Service.EventBus.Producers
{
    public class TwitchEventSubProducer : IHostedService
    {
        private readonly IEventBus _bus;
        private readonly ILogger<TwitchEventSubProducer> _logger;

        private readonly TwitchOptions _options;
        private readonly EventSubWebsocketClient _eventSubWebsocketClient;
        private readonly TwitchClient _twitch;
        private readonly string _userId;

        private const string BROADCASTER_ID = "broadcaster_user_id";
        private const string MODERATOR_ID = "moderator_user_id";
        private const string USER_ID = "user_id";


        public TwitchEventSubProducer(
            IEventBus bus,
            ILogger<TwitchEventSubProducer> logger,
            IOptions<TwitchOptions> options,
            TwitchClient twitchClient,
            EventSubWebsocketClient eventSubWebsocketClient)
        {
            _bus = bus;
            _logger = logger;
            _options = options.Value;
            _twitch = twitchClient;
            _eventSubWebsocketClient = eventSubWebsocketClient;
            _userId = _options.ChannelId;

            // Connection Hooks
            _eventSubWebsocketClient.WebsocketConnected += OnWebsocketConnected;
            _eventSubWebsocketClient.WebsocketDisconnected += OnWebsocketDisconnected;
            _eventSubWebsocketClient.WebsocketReconnected += OnWebsocketReconnected;
            _eventSubWebsocketClient.ErrorOccurred += OnErrorOccurred;

            // Message Hooks
            _eventSubWebsocketClient.ChannelChatMessage += OnChannelChatMessage;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _eventSubWebsocketClient.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _eventSubWebsocketClient.DisconnectAsync();
        }

        private async Task OnWebsocketConnected(object? sender, WebsocketConnectedArgs e)
        {
            _logger.LogInformation("Websocket {SessionId} connected!", _eventSubWebsocketClient.SessionId);

            if (!e.IsRequestedReconnect)
            {
                // https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/
                await SubscribeTo("channel.chat.message", "1", [BROADCASTER_ID, USER_ID]);
                await SubscribeTo("channel.follow", "2", [BROADCASTER_ID, MODERATOR_ID]);
                await SubscribeTo("channel.poll.begin", "1", [BROADCASTER_ID]);
                await SubscribeTo("channel.poll.end", "1", [BROADCASTER_ID]);
                await SubscribeTo("channel.prediction.begin", "1", [BROADCASTER_ID]);
                await SubscribeTo("channel.prediction.progress", "1", [BROADCASTER_ID]);
                await SubscribeTo("channel.prediction.lock", "1", [BROADCASTER_ID]);
                await SubscribeTo("channel.prediction.end", "1", [BROADCASTER_ID]);
                await SubscribeTo("channel.update", "2", [BROADCASTER_ID]);
            }
        }

        private Task<CreateEventSubSubscriptionResponse> SubscribeTo(string type, string version, List<string> conditions)
        {
            return _twitch.API.EventSub.CreateEventSubSubscriptionAsync(
                    type: type,
                    version: version,
                    condition: conditions.ToDictionary(x => x, x => _userId),
                    method: EventSubTransportMethod.Websocket,
                    websocketSessionId: _eventSubWebsocketClient.SessionId
                );
        }

        private async Task OnWebsocketDisconnected(object? sender, WebsocketDisconnectedArgs e)
        {
            _logger.LogError("Websocket {SessionId} disconnected!", _eventSubWebsocketClient.SessionId);

            while (!await _eventSubWebsocketClient.ReconnectAsync())
            {
                _logger.LogError("Websocket reconnect failed!");
                await Task.Delay(1000);
            }
        }

        private async Task OnWebsocketReconnected(object? sender, WebsocketReconnectedArgs e)
        {
            _logger.LogWarning("Websocket {SessionId} reconnected", _eventSubWebsocketClient.SessionId);
        }

        private async Task OnErrorOccurred(object? sender, ErrorOccuredArgs e)
        {
            _logger.LogError("Websocket {SessionId} - Error occurred!", _eventSubWebsocketClient.SessionId);
        }

        private async Task OnChannelChatMessage(object? sender, ChannelChatMessageArgs e)
        {
            var twitchMessageEvent = new ChatReceivedEvent(
                id: e.Payload.Event.MessageId,
                eventSource: EventSource.Twitch,
                message: e.Payload.Event.Message.Text,
                user: e.Payload.Event.ChatterUserName
            );

            await _bus.Publish(twitchMessageEvent);
        }
    }
}
