using Hellbot.Core.Events;
using Hellbot.Core.Events.Test;
using Hellbot.Service.EventBus.Handlers.Global;

namespace Hellbot.Service.EventBus.Handlers.Test
{
    public class TestMessageHandler
    {
        private readonly ILogger<EventLogger> _logger;

        public TestMessageHandler(IEventBus bus, ILogger<EventLogger> logger)
        {
            _logger = logger;
            bus.Subscribe<TestMessageEvent>(Handle);
        }

        private Task Handle(TestMessageEvent evt)
        {
            _logger.LogInformation($"Handled Test Message: {evt.Message}");
            return Task.CompletedTask;
        }
    }
}
