using Hellbot.Core.Events;
using Hellbot.Core.Users;
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
            UserIdentity? sender = evt.Context.Sender?.Identity;
            if (sender is null && evt.Context.TryGetUser(out var user))
                sender = new UserIdentity { Platform = PlatformSource.Hellbot, UserId = user.Id.ToString() };

            var message = new HubEventMessage
            {
                Id = evt.Id,
                Type = evt.GetType().Name,
                Timestamp = evt.Timestamp,
                Source = evt.Source,
                User = sender,
                Data = JsonSerializer.SerializeToElement(dataValue, dataValue.GetType()),
            };

            return hubContext.Clients.All.SendAsync("ReceiveEvent", message);
        }
    }
}
