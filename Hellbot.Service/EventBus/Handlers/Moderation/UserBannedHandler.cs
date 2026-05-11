using Hellbot.Core.Events;
using Hellbot.Core.Events.Moderation;
using Hellbot.Core.Users;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Users;
using Hellbot.Service.Users.Identity;

namespace Hellbot.Service.EventBus.Handlers.Moderation
{
    public class UserBannedHandler(IUserService userService, ILogger<UserBannedHandler> logger) : EventHandlerBase<UserBanned>
    {
        public override async Task Handle(UserBanned evt)
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
                            "User banned event {EventId}: resolution pointed to missing Hellbot user {HellbotUserId}.",
                            evt.Id,
                            resolved.HellbotUserId);
                        return;
                    }

                    if (string.Equals(user.Status, Standing.Banned, StringComparison.Ordinal))
                        return;

                    await userService.UpdateAsync(user with { Status = Standing.Banned });
                    logger.LogInformation(
                        "User banned: EventId={EventId} HellbotUserId={HellbotUserId} PlatformUserId={PlatformUserId} BannedAt={BannedAt} Reason={Reason} Source={Source}",
                        evt.Id,
                        user.Id,
                        evt.Data.UserId,
                        evt.Data.BannedAt,
                        evt.Data.Reason,
                        evt.Source);
                    break;
                }
                case UserResolutionResult.AmbiguousUsername ambiguous:
                    logger.LogWarning(
                        "User banned event {EventId}: unexpected ambiguous resolution for platform account locator; candidates={CandidateHellbotUserIds}",
                        evt.Id,
                        ambiguous.CandidateHellbotUserIds);
                    break;
                case UserResolutionResult.NotFound:
                    logger.LogInformation(
                        "User banned: no Hellbot user for {Platform} account {PlatformUserId}; EventId={EventId}",
                        evt.Source.Platform,
                        evt.Data.UserId,
                        evt.Id);
                    break;
            }
        }
    }
}
