using Hellbot.Core.Events;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Hellbot.Service.EventBus.Handlers.Global
{
    public class SignalREventBroadcaster(IHubContext<EventHub> hubContext) : IEventHandler
    {
        public bool CanHandle(IHellbotEvent evt) => true;

        public Task Handle(IHellbotEvent evt)
        {
            var eventType = evt.GetType();

            var dataProperty = eventType.GetProperty("Data");
            var dataValue = dataProperty?.GetValue(evt)!;

            var envelope = new
            {
                id = evt.Id,
                type = evt.GetType().Name,
                timestamp = evt.Timestamp,
                source = new { platform = evt.Source.Platform, channel = evt.Source.Channel },
                data = JsonSerializer.SerializeToElement(dataValue, dataValue.GetType())
            };

            return hubContext.Clients.All.SendAsync("ReceiveEvent", envelope);
        }
    }
}
