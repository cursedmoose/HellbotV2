using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;
using Hellbot.Service.Clients.Twitch;
using Hellbot.Service.Config;
using Microsoft.Extensions.Options;
using TwitchLib.Api.Core.Enums;
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
            _logger.LogInformation($"Websocket {_eventSubWebsocketClient.SessionId} connected!");

            if (!e.IsRequestedReconnect)
            {
                // Create and send EventSubscription
                await _twitch.API.EventSub.CreateEventSubSubscriptionAsync(
                    type: "channel.chat.message",
                    version: "1",
                    condition: new Dictionary<string, string> { 
                        { "broadcaster_user_id", _userId }, 
                        { "user_id", _userId } 
                    },
                    EventSubTransportMethod.Websocket,
                    _eventSubWebsocketClient.SessionId
                );
                // If you want to get Events for special Events you need to additionally add the AccessToken of the ChannelOwner to the request.
                // https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/

                await _twitch.API.EventSub.CreateEventSubSubscriptionAsync(
                    type: "channel.follow",
                    version: "2",
                    condition: new Dictionary<string, string>
                    {
                        { "broadcaster_user_id", _userId },
                        { "moderator_user_id", _userId },
                    },
                    method: EventSubTransportMethod.Websocket,
                    websocketSessionId: _eventSubWebsocketClient.SessionId
                );
            }
        }

        private async Task OnWebsocketDisconnected(object? sender, WebsocketDisconnectedArgs e)
        {
            _logger.LogError($"Websocket {_eventSubWebsocketClient.SessionId} disconnected!");

            // Don't do this in production. You should implement a better reconnect strategy with exponential backoff
            while (!await _eventSubWebsocketClient.ReconnectAsync())
            {
                _logger.LogError("Websocket reconnect failed!");
                await Task.Delay(1000);
            }
        }

        private async Task OnWebsocketReconnected(object? sender, WebsocketReconnectedArgs e)
        {
            _logger.LogWarning($"Websocket {_eventSubWebsocketClient.SessionId} reconnected");
        }

        private async Task OnErrorOccurred(object? sender, ErrorOccuredArgs e)
        {
            _logger.LogError($"Websocket {_eventSubWebsocketClient.SessionId} - Error occurred!");
        }

        private async Task OnChannelChatMessage(object? sender, ChannelChatMessageArgs e)
        {
            var twitchMessageEvent = new ChatReceivedEvent(
                eventSource: "Twitch",
                message: e.Payload.Event.Message.Text,
                user: e.Payload.Event.ChatterUserName
            );

            await _bus.Publish(twitchMessageEvent);
        }
    }
}
