using Hellbot.Core.Events;
using Hellbot.Core.Events.Users;
using Hellbot.Core.Users;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Users
{
    public class UserFollowedHandler(IUserService userService) : EventHandlerBase<UserFollowed>
    {
        public override Task Handle(UserFollowed evt)
        {
            UserIdentity identity = evt.Context.User?.Identity
                ?? new UserIdentity
                {
                    Platform = PlatformSource.Twitch,
                    UserId = evt.Data.FollowerUserId,
                    Username = evt.Data.FollowerUserName
                };

            return userService.UpdateUserRoleAsync(identity, Role.Member);
        }
    }
}
