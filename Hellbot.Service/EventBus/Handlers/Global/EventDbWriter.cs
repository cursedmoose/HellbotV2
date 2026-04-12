using Hellbot.Core.Events;
using Hellbot.Service.Data.Tables;

namespace Hellbot.Service.EventBus.Handlers.Global
{
    public class EventDbWriter(EventTable db): IEventHandler
    {
        public bool CanHandle(IHellbotEvent evt) => true;

        public async Task Handle(IHellbotEvent evt)
        {
            if (evt.StreamId != null || true)
            {
                await db.InsertAsync(evt);
            }
        }
    }
}
