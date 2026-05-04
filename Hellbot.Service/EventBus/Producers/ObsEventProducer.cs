using Hellbot.Core.Events;
using Hellbot.Core.Events.Session;
using Hellbot.Service.Clients.OBS;

namespace Hellbot.Service.EventBus.Producers
{
    public class ObsEventProducer(IEventBus bus, ObsClient obs, ILogger<ObsEventProducer> logger) : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            obs.Start();

            obs.API.Connected += (o, s) => PublishWebsocketStatus(ConnectionState.Connected, null);
            obs.API.Disconnected += (o, s) => PublishWebsocketStatus(ConnectionState.Disconnected, null);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            obs.Stop();
            return Task.CompletedTask;
        }

        private Task PublishWebsocketStatus(ConnectionState state, string? details)
        {
            return bus.Publish(new WebsocketStateChanged
            {
                Data = new()
                {
                    Status = state,
                    Details = details
                },
                Source = EventSource.OBS
            });
        }
    }
}
