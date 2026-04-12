using Hellbot.Core.Events;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Middleware
{
    public class UserContextEnricher(IUserResolver userResolver) : IEventMiddleware
    {
        public async Task Invoke(IHellbotEvent evt)
        {
            if (evt.Context.UserContext != null)
            {
                var context = evt.Context.UserContext;
                var user = await userResolver.Resolve(context.Identity.UserId, context.Identity.Platform);
                evt.Context.UserContext.Info = user;
            }

            return;
        }
    }
}
