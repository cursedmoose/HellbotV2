using Hellbot.Core.Events;
using Hellbot.Core.Events.Moderation;
using Hellbot.Service.EventBus.Handlers;

namespace Hellbot.Service.EventBus.Handlers.Moderation
{
    public class UserUnbannedHandler(ILogger<UserUnbannedHandler> logger) : EventHandlerBase<UserUnbanned>
    {
        public override Task Handle(UserUnbanned evt)
        {
            logger.LogInformation(
                "User unbanned: EventId={EventId} UserId={UserId} Source={Source}",
                evt.Id,
                evt.Data.UserId,
                evt.Source);
            return Task.CompletedTask;
        }
    }
}
