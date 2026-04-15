using Hellbot.Core.Events;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Middleware
{
    public class UserContextEnricher(IUserService userService) : IEventMiddleware
    {
        public async Task Invoke(IHellbotEvent evt)
        {
            if (evt.Context.User is UserContext context)
            {
                var user = await userService.GetOrCreateUser(context.Identity);
                context.Info = user;
            }

            return;
        }
    }
}
