using Hellbot.Core.Events;
using Hellbot.Service.Users;
using Hellbot.Service.Users.Identity;

namespace Hellbot.Service.EventBus.Middleware
{
    public class UserContextEnricher(IUserService userService) : IEventMiddleware
    {
        public async Task Invoke(IHellbotEvent evt)
        {
            if (evt.Context.User is UserContext uc)
            {
                var result = await userService.ResolveAsync(uc.Locator);
                if (result is UserResolutionResult.Resolved resolved)
                {
                    var user = await userService.GetAsync(resolved.HellbotUserId);
                    evt.Context = evt.Context with
                    {
                        User = uc with { Info = user },
                    };
                }
            }

            return;
        }
    }
}
