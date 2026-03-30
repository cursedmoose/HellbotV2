using Hellbot.Core.Events;

namespace Hellbot.Service.EventBus.Handlers.Global
{
    public class EventLogger
    {
        private readonly ILogger<EventLogger> _logger;
        public EventLogger(IEventBus bus, ILogger<EventLogger> logger)  
        {
            bus.SubscribeAll(Handle);
            _logger = logger;
            _logger.LogInformation($"Logger got bus: {bus.GetHashCode()}");
        }
        private Task Handle(IHellbotEvent evt)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(evt);

            _logger.LogInformation($"[{evt.Timestamp}] {evt.GetType().Name}: {json}");

            return Task.CompletedTask;
        }
    }
}
