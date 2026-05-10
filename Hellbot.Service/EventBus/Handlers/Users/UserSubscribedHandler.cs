using Hellbot.Core.Events;
using Hellbot.Core.Events.Users;
using Hellbot.Core.Events.Context;
using Hellbot.Core.Users;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Users
{
    public class UserSubscribedHandler(IUserService userService) : EventHandlerBase<UserSubscribed>
    {
        public override async Task Handle(UserSubscribed evt)
        {
            if (evt.Context.User is not UserContext uc)
                return;

            await userService.TryUpgradeRoleAsync(uc.Info!.Id, Role.Premium);
        }
    }
}
