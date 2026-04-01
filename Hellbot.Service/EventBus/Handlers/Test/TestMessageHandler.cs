using Hellbot.Core.Events;
using Hellbot.Core.Events.Test;

namespace Hellbot.Service.EventBus.Handlers.Test
{
    public class TestMessageHandler(ILogger<TestMessageHandler> logger) : IEventHandler
    {
        public void Register(IEventBus bus)
        {
            bus.Subscribe<TestMessageEvent>(Handle);
        }

        private Task Handle(TestMessageEvent evt)
        {
            logger.LogInformation($"Handled Test Message: {evt.Message}");
            return Task.CompletedTask;
        }
    }
}
