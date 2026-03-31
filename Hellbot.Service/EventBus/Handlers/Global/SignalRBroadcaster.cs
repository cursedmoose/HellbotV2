using Hellbot.Core.Events;
using Microsoft.AspNetCore.SignalR;

namespace Hellbot.Service.EventBus.Handlers.Global
{
    public class SignalREventBroadcaster: IEventHandler
    {
        IHubContext<EventHub> _hubContext;
        public SignalREventBroadcaster(IHubContext<EventHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void Register(IEventBus bus)
        {
            bus.SubscribeAll(evt => Broadcast(evt));
        }

        private Task Broadcast(IHellbotEvent evt)
        {
            return _hubContext.Clients.All.SendAsync("ReceiveEvent", new
            {
                type = evt.GetType().Name,
                timestamp = evt.Timestamp,
                data = evt
            });
        }
    }
}
