using Hellbot.Core.Events;
using Hellbot.Core.Events.Moderation;
using Hellbot.Service.EventBus.Handlers;

namespace Hellbot.Service.EventBus.Handlers.Moderation
{
    public class UserBannedHandler(ILogger<UserBannedHandler> logger) : EventHandlerBase<UserBanned>
    {
        public override Task Handle(UserBanned evt)
        {
            logger.LogInformation(
                "User banned: EventId={EventId} UserId={UserId} BannedAt={BannedAt} Reason={Reason} Source={Source}",
                evt.Id,
                evt.Data.UserId,
                evt.Data.BannedAt,
                evt.Data.Reason,
                evt.Source);
            return Task.CompletedTask;
        }
    }
}
