using Hellbot.Core.Events;
using Hellbot.Core.Events.Test;

namespace Hellbot.Service.EventBus.Handlers.Test
{
    public class TestMessageHandler(ILogger<TestMessageHandler> logger) : IEventHandler
    {
        public void Register(IEventBus bus)
        {
            bus.Subscribe<TestEvent>(Handle);
        }

        private Task Handle(TestEvent evt)
        {
            logger.LogInformation("Handled Test Message: {Message}", evt.Data.Message);
            return Task.CompletedTask;
        }
    }
}
