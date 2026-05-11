using Hellbot.Core.Events;
using Hellbot.Core.Events.Moderation;
using Hellbot.Core.Users;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Users;
using Hellbot.Service.Users.Identity;

namespace Hellbot.Service.EventBus.Handlers.Moderation
{
    public class UserUnmutedHandler(IUserService userService, ILogger<UserUnmutedHandler> logger) : EventHandlerBase<UserUnmuted>
    {
        public override async Task Handle(UserUnmuted evt)
        {
            var locator = new UserLocator.PlatformAccount(evt.Source.Platform, evt.Data.UserId);
            var result = await userService.ResolveAsync(locator);
            switch (result)
            {
                case UserResolutionResult.Resolved resolved:
                {
                    var user = await userService.GetAsync(resolved.HellbotUserId);
                    if (user is null)
                    {
                        logger.LogWarning(
                            "User unmuted event {EventId}: resolution pointed to missing Hellbot user {HellbotUserId}.",
                            evt.Id,
                            resolved.HellbotUserId);
                        return;
                    }

                    if (string.Equals(user.Status, Standing.Banned, StringComparison.Ordinal))
                        return;

                    if (string.Equals(user.Status, Standing.Active, StringComparison.Ordinal))
                        return;

                    await userService.UpdateAsync(user with { Status = Standing.Active });
                    logger.LogInformation(
                        "User unmuted: EventId={EventId} HellbotUserId={HellbotUserId} PlatformUserId={PlatformUserId} Source={Source}",
                        evt.Id,
                        user.Id,
                        evt.Data.UserId,
                        evt.Source);
                    break;
                }
                case UserResolutionResult.AmbiguousUsername ambiguous:
                    logger.LogWarning(
                        "User unmuted event {EventId}: unexpected ambiguous resolution for platform account locator; candidates={CandidateHellbotUserIds}",
                        evt.Id,
                        ambiguous.CandidateHellbotUserIds);
                    break;
                case UserResolutionResult.NotFound:
                    logger.LogInformation(
                        "User unmuted: no Hellbot user for {Platform} account {PlatformUserId}; EventId={EventId}",
                        evt.Source.Platform,
                        evt.Data.UserId,
                        evt.Id);
                    break;
            }
        }
    }
}
