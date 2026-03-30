using Hellbot.Core.Events;
using Microsoft.AspNetCore.SignalR;

namespace Hellbot.Service.EventBus
{
    public class SignalREventBroadcaster
    {
        public SignalREventBroadcaster(
            IEventBus bus,
            IHubContext<EventHub> hubContext)
        {
            bus.SubscribeAll(evt => Broadcast(evt, hubContext));
        }

        private Task Broadcast(IHellbotEvent evt, IHubContext<EventHub> hub)
        {
            return hub.Clients.All.SendAsync("ReceiveEvent", new
            {
                type = evt.GetType().Name,
                timestamp = evt.Timestamp,
                data = evt
            });
        }
    }
}
