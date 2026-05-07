using Hellbot.Core.Commands;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Users;
using Hellbot.Core.Users;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Data.Tables.Users;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Users
{
    public class UserFollowedHandler(
        IUserService userService,
        UserTable users,
        UserCache cache) : EventHandlerBase<UserFollowed>
    {
        public override async Task Handle(UserFollowed evt)
        {
            UserIdentity identity = evt.Context.User?.Identity
                ?? new UserIdentity
                {
                    Platform = PlatformSource.Twitch,
                    UserId = evt.Data.FollowerUserId,
                    Username = evt.Data.FollowerUserName
                };

            var user = await userService.GetOrCreateUser(identity);
            if (user.Role >= Role.Member)
                return;

            var updated = user with { Role = Role.Member };
            await users.Update(updated);
            cache.SetUser(updated);
        }
    }
}
