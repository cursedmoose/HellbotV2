using Hellbot.Core.Events;
using Hellbot.Service.Data;

namespace Hellbot.Service.EventBus.Handlers.Global
{
    public class EventDbWriter(EventTable db): IEventHandler
    {
        public Task Handle(IHellbotEvent evt) => db.InsertAsync(evt);

        public void Register(IEventBus bus)
        {
            bus.Subscribe<IHellbotEvent>(Handle);
        }
    }
}
