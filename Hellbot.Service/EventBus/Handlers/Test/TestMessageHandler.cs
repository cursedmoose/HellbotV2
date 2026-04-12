using Hellbot.Core.Events.Test;

namespace Hellbot.Service.EventBus.Handlers.Test
{
    public class TestMessageHandler(ILogger<TestMessageHandler> logger) : EventHandlerBase<TestEvent>
    {
        public override Task Handle(TestEvent evt)
        {
            logger.LogInformation("Handled Test Message: {Message}", evt.Data.Message);
            return Task.CompletedTask;
        }
    }
}
