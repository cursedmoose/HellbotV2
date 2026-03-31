using Hellbot.Core.Events;

namespace Hellbot.Service.EventBus.Handlers.Global
{
    public class EventLogger: IEventHandler
    {
        private readonly ILogger<EventLogger> _logger;
        public EventLogger(ILogger<EventLogger> logger)
        {
            _logger = logger;
        }

        public void Register(IEventBus bus)
        {
            bus.SubscribeAll(Handle);
            _logger.LogInformation($"Logger initialized onto bus.");
        }

        private Task Handle(IHellbotEvent evt)
        {
            _logger.LogInformation(
                "Event received: {EventType} {@Event}",
                evt.GetType().Name,
                evt
            );

            return Task.CompletedTask;
        }
    }
}
