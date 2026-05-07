using Hellbot.Core.Commands;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Users;
using Hellbot.Core.Users;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Data.Tables.Users;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Users
{
    public class UserSubscribedHandler(
        IUserService userService,
        UserTable users,
        UserCache cache) : EventHandlerBase<UserSubscribed>
    {
        public override async Task Handle(UserSubscribed evt)
        {
            UserIdentity identity = evt.Context.User?.Identity
                ?? new UserIdentity
                {
                    Platform = PlatformSource.Twitch,
                    UserId = evt.Data.SubscriberUserId,
                    Username = evt.Data.SubscriberUserName
                };

            var user = await userService.GetOrCreateUser(identity);
            if (user.Role >= Role.Premium)
                return;

            var updated = user with { Role = Role.Premium };
            await users.Update(updated);
            cache.SetUser(updated);
        }
    }
}
