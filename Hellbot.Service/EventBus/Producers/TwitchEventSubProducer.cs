using Hellbot.Core.Commands;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;
using Hellbot.Core.Events.Users;
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
            _eventSubWebsocketClient.ChannelChatMessageDelete += OnChannelChatMessageDelete;

            // Moderation Hooks
            _eventSubWebsocketClient.ChannelBan += OnChannelBan;
            _eventSubWebsocketClient.ChannelUnban += OnChannelUnban;
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

                await SubscribeTo("channel.subscribe", "1", [BROADCASTER_ID]);
                await SubscribeTo("channel.subscription.gift", "1", [BROADCASTER_ID]);
                await SubscribeTo("channel.follow", "2", [BROADCASTER_ID, MODERATOR_ID]);

                await SubscribeTo("channel.poll.begin", "1", [BROADCASTER_ID]);
                await SubscribeTo("channel.poll.progress", "1", [BROADCASTER_ID]);
                await SubscribeTo("channel.poll.end", "1", [BROADCASTER_ID]);

                await SubscribeTo("channel.prediction.begin", "1", [BROADCASTER_ID]);
                await SubscribeTo("channel.prediction.progress", "1", [BROADCASTER_ID]);
                await SubscribeTo("channel.prediction.lock", "1", [BROADCASTER_ID]);
                await SubscribeTo("channel.prediction.end", "1", [BROADCASTER_ID]);

                await SubscribeTo("channel.update", "2", [BROADCASTER_ID]);

                await SubscribeTo("stream.online", "1", [BROADCASTER_ID]);
                await SubscribeTo("stream.offline", "1", [BROADCASTER_ID]);
            }
        }

        private async Task<CreateEventSubSubscriptionResponse> SubscribeTo(string type, string version, List<string> conditions)
        {
            try
            {
                return await _twitch.API.EventSub.CreateEventSubSubscriptionAsync(
                        type: type,
                        version: version,
                        condition: conditions.ToDictionary(x => x, x => _userId),
                        method: EventSubTransportMethod.Websocket,
                        websocketSessionId: _eventSubWebsocketClient.SessionId
                    );
            }
            catch (TwitchLib.Api.Core.Exceptions.BadTokenException e)
            {
                _logger.LogWarning("{Exception}: Probably Missing scope for {SubscriptionType}", e.InnerException, type);
                return new();
            }
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
            _logger.LogError("Websocket {SessionId} - Error occurred! {Error}:{Message}", _eventSubWebsocketClient.SessionId, e.Exception, e.Message);
        }

        private static Role GetDefaultRole(ChannelChatMessageArgs e)
        {
            return e.Payload.Event switch
            {
                { IsBroadcaster: true } => Role.Admin,
                { IsSubscriber: true } => Role.Premium,
                { IsVip: true } => Role.Premium,
                _ => Role.None
            };
        }

        private static bool TryParseCommand(string message, out string command, out string[] args)
        {
            command = null!;
            args = [];

            if (string.IsNullOrWhiteSpace(message) || message[0] != '!')
                return false;

            var parts = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var cmd = parts[0][1..];

            if (string.IsNullOrWhiteSpace(cmd) || !char.IsLetter(cmd[0]))
                return false;

            if (!cmd.All(char.IsLetterOrDigit))
                return false;

            command = cmd;
            args = [..parts.Skip(1)];
            return true;
        }

        private async Task OnChannelChatMessage(object? sender, ChannelChatMessageArgs e)
        {
            var message = e.Payload.Event.Message.Text;
            IHellbotEvent hellbotEvent;
            if (TryParseCommand(message, out string command, out string[] commandArgs))
            {
                hellbotEvent = new CommandRequested
                {
                    Data = new()
                    {
                        Command = command,
                        CommandArgs = commandArgs,
                        CommandSource = EventSource.Twitch,
                        User = e.Payload.Event.ChatterUserId,
                        UserRole = GetDefaultRole(e)
                    },
                    Source = EventSource.Twitch
                };
            }
            else
            {

                hellbotEvent = new ChatMessageReceived
                {
                    Data = new()
                    {
                        User = e.Payload.Event.ChatterUserId,
                        Message = e.Payload.Event.Message.Text,
                        MessageId = e.Payload.Event.MessageId,
                    },
                    Source = EventSource.Twitch
                };
            }

            await _bus.Publish(hellbotEvent);
        }

        private async Task OnChannelChatMessageDelete(object? sender, ChannelChatMessageDeleteArgs e) 
        {
            var hellbotEvent = new ChatMessageDeleted
            {
                Data = new()
                {
                    MessageId = e.Payload.Event.MessageId
                },
                Source = EventSource.Twitch
            };

            await _bus.Publish(hellbotEvent);
        }

        private async Task OnChannelBan(object? sender, ChannelBanArgs e)
        {
            var hellbotEvent = new UserBanned
            {
                Data = new()
                {
                    UserId = e.Payload.Event.UserId,
                    Reason = e.Payload.Event.Reason,
                    IsPermanent = e.Payload.Event.IsPermanent,
                    BannedAt = e.Payload.Event.BannedAt
                },
                Source = EventSource.Twitch,
            };

            await _bus.Publish(hellbotEvent);
        }

        private async Task OnChannelUnban(object? sender, ChannelUnbanArgs e)
        {
            var hellbotEvent = new UserUnbanned
            {
                Data = new()
                {
                    UserId = e.Payload.Event.UserId
                },
                Source = EventSource.Twitch,
            };

            await _bus.Publish(hellbotEvent);
        }
    }
}
