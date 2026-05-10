using Hellbot.Core.Events;
using Hellbot.Service.Users;
using Hellbot.Service.Users.Identity;

namespace Hellbot.Service.EventBus.Middleware
{
    public class UserContextEnricher(IUserService userService, ILogger<UserContextEnricher> logger) : IEventMiddleware
    {
        public async Task Invoke(IHellbotEvent evt)
        {
            if (evt.Context.User is not UserContext uc)
                return;

            var result = await userService.ResolveAsync(uc.Locator);
            switch (result)
            {
                case UserResolutionResult.Resolved resolved:
                {
                    var user = await userService.GetAsync(resolved.HellbotUserId);
                    if (user is null)
                    {
                        logger.LogWarning(
                            "User resolution succeeded for locator {Locator} but user {HellbotUserId} was not found.",
                            uc.Locator, resolved.HellbotUserId);
                        break;
                    }

                    evt.Context = evt.Context with
                    {
                        User = uc with { Info = user },
                    };

                    break;
                }
                case UserResolutionResult.AmbiguousUsername ambiguous:
                    logger.LogWarning(
                        "Ambiguous username for locator {Locator}; candidate Hellbot user ids: {CandidateHellbotUserIds}",
                        uc.Locator, ambiguous.CandidateHellbotUserIds);
                    break;

                case UserResolutionResult.NotFound:
                    break;
            }
        }
    }
}
