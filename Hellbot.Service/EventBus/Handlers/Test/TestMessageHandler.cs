using Hellbot.Core.Events;
using Hellbot.Core.Events.Test;
using Hellbot.Service.EventBus.Handlers.Global;

namespace Hellbot.Service.EventBus.Handlers.Test
{
    public class TestMessageHandler: IEventHandler
    {
        private readonly ILogger<TestMessageHandler> _logger;

        public TestMessageHandler(ILogger<TestMessageHandler> logger)
        {
            _logger = logger;
        }

        public void Register(IEventBus bus)
        {
            bus.Subscribe<TestMessageEvent>(Handle);
            _logger.LogInformation("TestMessage Handler initialized.");
        }

        private Task Handle(TestMessageEvent evt)
        {
            _logger.LogInformation($"Handled Test Message: {evt.Message}");
            return Task.CompletedTask;
        }
    }
}
