using Hellbot.Core.Events;
using Hellbot.Core.Events.Test;

namespace Hellbot.Service.EventBus.Producers
{
    public class HeartbeatProducer: BackgroundService
    {
        private readonly IEventBus _bus;

        public HeartbeatProducer(IEventBus bus)
        {
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // fake message
                var evt = new TestMessageEvent
                {
                    Message = "PING HEARTBEAT"
                };

                await _bus.PublishAsync(evt);

                await Task.Delay(60_000, stoppingToken);
            }
        }
    }
}
