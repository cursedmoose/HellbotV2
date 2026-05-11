using Hellbot.Core.Events;
using Hellbot.Core.Events.Moderation;
using Hellbot.Core.Users;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Users;
using Hellbot.Service.Users.Identity;

namespace Hellbot.Service.EventBus.Handlers.Moderation
{
    public class UserMutedHandler(IUserService userService, ILogger<UserMutedHandler> logger) : EventHandlerBase<UserMuted>
    {
        public override async Task Handle(UserMuted evt)
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
                            "User muted event {EventId}: resolution pointed to missing Hellbot user {HellbotUserId}.",
                            evt.Id,
                            resolved.HellbotUserId);
                        return;
                    }

                    if (string.Equals(user.Status, Standing.Banned, StringComparison.Ordinal))
                        return;

                    if (string.Equals(user.Status, Standing.Muted, StringComparison.Ordinal))
                        return;

                    await userService.UpdateAsync(user with { Status = Standing.Muted });
                    logger.LogInformation(
                        "User muted: EventId={EventId} HellbotUserId={HellbotUserId} PlatformUserId={PlatformUserId} ExpiresAt={ExpiresAt} Reason={Reason} Source={Source}",
                        evt.Id,
                        user.Id,
                        evt.Data.UserId,
                        evt.Data.ExpiresAt,
                        evt.Data.Reason,
                        evt.Source);
                    break;
                }
                case UserResolutionResult.AmbiguousUsername ambiguous:
                    logger.LogWarning(
                        "User muted event {EventId}: unexpected ambiguous resolution for platform account locator; candidates={CandidateHellbotUserIds}",
                        evt.Id,
                        ambiguous.CandidateHellbotUserIds);
                    break;
                case UserResolutionResult.NotFound:
                    logger.LogInformation(
                        "User muted: no Hellbot user for {Platform} account {PlatformUserId}; EventId={EventId}",
                        evt.Source.Platform,
                        evt.Data.UserId,
                        evt.Id);
                    break;
            }
        }
    }
}
