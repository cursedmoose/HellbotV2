using Hellbot.Core.Events;

namespace Hellbot.Service.EventBus.Handlers.Global
{
    public class EventLogger(ILogger<EventLogger> logger) : IEventHandler
    {
        public void Register(IEventBus bus)
        {
            bus.Subscribe<IHellbotEvent>(Handle);
        }

        private Task Handle(IHellbotEvent evt)
        {
            logger.LogInformation(
                "{EventType} {@Event}",
                evt.GetType().Name,
                evt
            );

            return Task.CompletedTask;
        }
    }
}
