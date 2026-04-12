using Hellbot.Core.Events;

namespace Hellbot.Service.EventBus.Handlers
{
    public abstract class EventHandlerBase<TEvent> : IEventHandler
    {
        public bool CanHandle(IHellbotEvent evt) => evt is TEvent;

        public abstract Task Handle(TEvent evt);

        async Task IEventHandler.Handle(IHellbotEvent evt) => await Handle((TEvent)evt);
    }
}
