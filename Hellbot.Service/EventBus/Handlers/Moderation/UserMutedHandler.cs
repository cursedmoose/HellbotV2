using Hellbot.Core.Events;
using Hellbot.Core.Events.Moderation;
using Hellbot.Service.EventBus.Handlers;

namespace Hellbot.Service.EventBus.Handlers.Moderation
{
    public class UserMutedHandler(ILogger<UserMutedHandler> logger) : EventHandlerBase<UserMuted>
    {
        public override Task Handle(UserMuted evt)
        {
            logger.LogInformation(
                "User muted: EventId={EventId} UserId={UserId} ExpiresAt={ExpiresAt} Reason={Reason} Source={Source}",
                evt.Id,
                evt.Data.UserId,
                evt.Data.ExpiresAt,
                evt.Data.Reason,
                evt.Source);
            return Task.CompletedTask;
        }
    }
}
