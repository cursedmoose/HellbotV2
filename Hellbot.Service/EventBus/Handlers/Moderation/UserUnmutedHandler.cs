using Hellbot.Core.Events;
using Hellbot.Core.Events.Moderation;
using Hellbot.Service.EventBus.Handlers;

namespace Hellbot.Service.EventBus.Handlers.Moderation
{
    public class UserUnmutedHandler(ILogger<UserUnmutedHandler> logger) : EventHandlerBase<UserUnmuted>
    {
        public override Task Handle(UserUnmuted evt)
        {
            logger.LogInformation(
                "User unmuted: EventId={EventId} UserId={UserId} Source={Source}",
                evt.Id,
                evt.Data.UserId,
                evt.Source);
            return Task.CompletedTask;
        }
    }
}
