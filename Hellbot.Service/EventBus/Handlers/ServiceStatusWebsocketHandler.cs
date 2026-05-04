using Hellbot.Core.Events;
using Hellbot.Core.Events.Session;
using Hellbot.Service.Status;

namespace Hellbot.Service.EventBus.Handlers
{
    public sealed class ServiceStatusWebsocketHandler(ServiceStatusProvider status) : IEventHandler
    {
        public bool CanHandle(IHellbotEvent evt) => evt is WebsocketStateChanged;

        public Task Handle(IHellbotEvent evt)
        {
            var changed = (WebsocketStateChanged)evt;
            status.RecordWebsocketStatus(changed.Source, changed.Data, changed.Timestamp);
            return Task.CompletedTask;
        }
    }
}
