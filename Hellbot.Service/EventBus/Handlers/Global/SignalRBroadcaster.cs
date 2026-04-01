using Hellbot.Core.Events;
using Microsoft.AspNetCore.SignalR;

namespace Hellbot.Service.EventBus.Handlers.Global
{
    public class SignalREventBroadcaster(IHubContext<EventHub> hubContext) : IEventHandler
    {
        public void Register(IEventBus bus)
        {
            bus.Subscribe<IHellbotEvent>(Broadcast);
        }

        private Task Broadcast(IHellbotEvent evt)
        {
            return hubContext.Clients.All.SendAsync("ReceiveEvent", new
            {
                type = evt.GetType().Name,
                timestamp = evt.Timestamp,
                data = evt
            });
        }
    }
}
