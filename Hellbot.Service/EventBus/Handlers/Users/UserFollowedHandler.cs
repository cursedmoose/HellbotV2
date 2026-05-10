using Hellbot.Core.Events;
using Hellbot.Core.Events.Users;
using Hellbot.Core.Events.Context;
using Hellbot.Core.Users;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Users
{
    public class UserFollowedHandler(IUserService userService) : EventHandlerBase<UserFollowed>
    {
        public override async Task Handle(UserFollowed evt)
        {
            if (evt.Context.User is not UserContext uc)
                return;

            await userService.TryUpgradeRoleAsync(uc.Info!.Id, Role.Member);
        }
    }
}
