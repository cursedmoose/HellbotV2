using Hellbot.Core.Events;
using Hellbot.Service.Clients.OBS;

namespace Hellbot.Service.EventBus.Producers
{
    public class ObsEventProducer(IEventBus bus, ObsClient obs, ILogger<ObsEventProducer> logger) : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            obs.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            obs.Stop();
            return Task.CompletedTask;
        }
    }
}
