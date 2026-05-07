using Hellbot.Core.Commands;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Users;
using Hellbot.Core.Users;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Users
{
    public class UserSubscribedHandler(IUserService userService) : EventHandlerBase<UserSubscribed>
    {
        public override Task Handle(UserSubscribed evt)
        {
            UserIdentity identity = evt.Context.User?.Identity
                ?? new UserIdentity
                {
                    Platform = PlatformSource.Twitch,
                    UserId = evt.Data.SubscriberUserId,
                    Username = evt.Data.SubscriberUserName
                };

            return userService.UpdateUserRoleAsync(identity, Role.Premium);
        }
    }
}
