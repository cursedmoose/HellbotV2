using Hellbot.Core.Events;
using Hellbot.Core.Events.Test;

namespace Hellbot.Service.EventBus.Producers
{
    public class HeartbeatProducer(IEventBus bus) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // fake message
                var evt = new TestMessageEvent
                {
                    Message = "PING HEARTBEAT"
                };

                await bus.Publish(evt);

                await Task.Delay(60_000, stoppingToken);
            }
        }
    }
}
