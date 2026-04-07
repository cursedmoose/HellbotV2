using Hellbot.Core.Events;

namespace Hellbot.Service.EventBus.Middleware
{
    public class EventLogger(ILogger<EventLogger> logger): IEventMiddleware
    {
        public async Task Invoke(IHellbotEvent evt)
        {
            logger.LogInformation(
                "{@Event}",
                evt.ToString()
            );
        }
    }
}
