using Hellbot.Core.Events;
using Hellbot.Core.Events.Users;
using Hellbot.Service.EventBus.Handlers;

namespace Hellbot.Service.EventBus.Handlers.Users
{
    public class UserModdedHandler(ILogger<UserModdedHandler> logger) : EventHandlerBase<UserModded>
    {
        public override Task Handle(UserModded evt)
        {
            logger.LogInformation(
                "User modded: EventId={EventId} UserId={UserId} Source={Source}",
                evt.Id,
                evt.Data.UserId,
                evt.Source);
            return Task.CompletedTask;
        }
    }
}
