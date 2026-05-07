using Hellbot.Core.Events;
using Hellbot.Service.Data.Tables;

namespace Hellbot.Service.EventBus.Handlers.Global
{
    public class EventDbWriter(EventTable db): IEventHandler
    {
        public bool CanHandle(IHellbotEvent evt) => true;

        public Task Handle(IHellbotEvent evt) => db.InsertAsync(evt);
    }
}
