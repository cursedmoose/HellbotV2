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
            var json = System.Text.Json.JsonSerializer.Serialize(evt);

            _logger.LogInformation($"[{evt.Timestamp}] {evt.GetType().Name}: {json}");

            return Task.CompletedTask;
        }
    }
}
