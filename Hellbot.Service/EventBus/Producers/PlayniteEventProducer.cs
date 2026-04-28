using Hellbot.Core.Events;
using Hellbot.Core.Events.Session;
using Hellbot.Service.Clients.Playnite;

namespace Hellbot.Service.EventBus.Producers
{
    public class PlayniteEventProducer : IHostedService
    {
        private readonly PlayniteWebsocket _ws;
        private readonly IEventBus _bus;
        public PlayniteEventProducer(IEventBus bus)
        {
            _ws = new PlayniteWebsocket("ws://127.0.0.1:6767");
            _bus = bus;

            _ws.Connected += () =>
            {
                Console.WriteLine("[Playnite] Connected");
            };

            _ws.Disconnected += () =>
            {
                Console.WriteLine("[Playnite] Disconnected");
            };

            _ws.GameStarted += e =>
            {
                Console.WriteLine($"[Playnite] Game started: {e.Game}");

                _bus.Publish(new GameStarted {
                    Data = new(),
                    Source = EventSource.Playnite
                });
            };

            _ws.RawMessage += msg =>
            {
                Console.WriteLine($"[Playnite RAW] {msg}");
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[Playnite] Starting client...");
            await _ws.Connect();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[Playnite] Stopping client...");
            await _ws.Disconnect();
        }
    }
}
