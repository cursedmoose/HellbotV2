using Hellbot.Core.Events;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Middleware
{
    public class UserContextEnricher(IUserService userService) : IEventMiddleware
    {
        public async Task Invoke(IHellbotEvent evt)
        {
            if (evt.Context.User is UserContext uc)
            {
                var user = await userService.GetOrCreateUser(uc.Identity);
                evt.Context = evt.Context with
                {
                    User = uc with { Info = user },
                };
            }

            return;
        }
    }
}
