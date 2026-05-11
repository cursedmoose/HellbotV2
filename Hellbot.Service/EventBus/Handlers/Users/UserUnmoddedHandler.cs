using Hellbot.Core.Events;
using Hellbot.Core.Events.Users;
using Hellbot.Service.EventBus.Handlers;

namespace Hellbot.Service.EventBus.Handlers.Users
{
    public class UserUnmoddedHandler(ILogger<UserUnmoddedHandler> logger) : EventHandlerBase<UserUnmodded>
    {
        public override Task Handle(UserUnmodded evt)
        {
            logger.LogInformation(
                "User unmodded: EventId={EventId} UserId={UserId} Source={Source}",
                evt.Id,
                evt.Data.UserId,
                evt.Source);
            return Task.CompletedTask;
        }
    }
}
