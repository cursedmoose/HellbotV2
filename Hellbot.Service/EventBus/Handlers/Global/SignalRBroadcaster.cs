using Hellbot.Core.Events;
using Microsoft.AspNetCore.SignalR;

namespace Hellbot.Service.EventBus.Handlers.Global
{
    public class SignalREventBroadcaster(IHubContext<EventHub> hubContext) : IEventHandler
    {
        public bool CanHandle(IHellbotEvent evt) => true;

        public Task Handle(IHellbotEvent evt)
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
