using Hellbot.Core.Events;
using Hellbot.Core.Events.Users;
using Hellbot.Core.Users;
using Hellbot.Service.Users.Identity;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Users
{
    public class UserSubscribedHandler(IUserService userService) : EventHandlerBase<UserSubscribed>
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

            var ensured = await userService.EnsureUserAsync(identity);
            await userService.TryUpgradeRoleAsync(new UserLocator.HellbotUser(ensured.Id), Role.Premium);
        }
    }
}
