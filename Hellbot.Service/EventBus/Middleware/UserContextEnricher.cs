using Hellbot.Core.Events;
using Hellbot.Core.Events.Context;
using Hellbot.Core.Users;
using Hellbot.Service.Users;
using Hellbot.Service.Users.Identity;

namespace Hellbot.Service.EventBus.Middleware
{
    public class UserContextEnricher(IUserService userService, ILogger<UserContextEnricher> logger) : IEventMiddleware
    {
        public async Task Invoke(IHellbotEvent evt)
        {
            if (evt.Context.HasUserContext)
                return;

            UserContext uc = evt.Context.User is UserContext existing ? existing : default;

            if (evt.Context.Sender?.Locator is { } locator)
            {
                var result = await userService.ResolveAsync(locator);
                switch (result)
                {
                    case UserResolutionResult.Resolved resolved:
                    {
                        var user = await userService.GetAsync(resolved.HellbotUserId);
                        if (user is null)
                        {
                            logger.LogWarning(
                                "User resolution succeeded for locator {Locator} but user {HellbotUserId} was not found.",
                                locator, resolved.HellbotUserId);
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
                            locator, ambiguous.CandidateHellbotUserIds);
                        break;

                    case UserResolutionResult.NotFound:
                        break;
                }

                return;
            }

            if (evt.Context.Sender?.Identity is { } identity)
            {
                var user = await userService.GetOrCreateUserAsync(identity);
                evt.Context = evt.Context with { User = uc with { Info = user } };
            }
        }
    }
}
