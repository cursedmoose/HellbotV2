using Hellbot.Core.Events;
using Hellbot.Core.Events.Users;
using Hellbot.Core.Users;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Users
{
    public class UserFollowedHandler(IUserService userService) : EventHandlerBase<UserFollowed>
    {
        public override async Task Handle(UserFollowed evt)
        {
            UserIdentity identity = new()
            {
                Platform = PlatformSource.Twitch,
                UserId = evt.Data.FollowerUserId,
                Username = evt.Data.FollowerUserName
            };

            var ensured = await userService.GetOrCreateUserAsync(identity);
            await userService.TryUpgradeRoleAsync(new UserLocator.HellbotUser(ensured.Id), Role.Member);
        }
    }
}
