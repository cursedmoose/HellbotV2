using Hellbot.Core.Events;
using Hellbot.Core.Events.Session;
using Hellbot.Service.Clients.Playnite;
using PlayniteWebsocket.Client;

namespace Hellbot.Service.EventBus.Producers
{
    public class PlayniteEventProducer : IHostedService
    {
        private readonly PlayniteClient _client;
        private readonly PlayniteWebsocketClient _playnite;
        private readonly IEventBus _bus;
        private readonly ILogger<PlayniteEventProducer> _logger;
        public PlayniteEventProducer(PlayniteClient playnite, IEventBus bus, ILogger<PlayniteEventProducer> logger)
        {
            _client = playnite;
            _playnite = playnite.Websocket;
            _bus = bus;
            _logger = logger;

            _playnite.GameStarted += evt =>
            {
                _logger.LogInformation("Started: {Game}", evt.GameName);
                _bus.Publish(new GameStarted
                {
                    Data = new GameStartedPayload
                    {
                        Id = evt.GameId,
                        Name = evt.GameName 
                    },
                    Source = EventSource.Playnite
                });
            };

            _playnite.GameStopped += evt =>
            {
                _logger.LogInformation("Stopped: {Game}", evt.GameName);
                _bus.Publish(new GameStopped
                {
                    Data = new GameStoppedPayload
                    {
                        Id = evt.GameId,
                        Name = evt.GameName
                    },
                    Source = EventSource.Playnite
                });
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting client...");
            await _client.Connect();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping client...");
            await _client.Disconnect();
        }
    }
}
