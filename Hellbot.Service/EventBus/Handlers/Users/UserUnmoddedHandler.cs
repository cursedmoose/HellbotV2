using Hellbot.Core.Events;
using Hellbot.Core.Events.Users;
using Hellbot.Core.Users;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Users;
using Hellbot.Service.Users.Identity;

namespace Hellbot.Service.EventBus.Handlers.Users
{
    public class UserUnmoddedHandler(IUserService userService, ILogger<UserUnmoddedHandler> logger) : EventHandlerBase<UserUnmodded>
    {
        public override async Task Handle(UserUnmodded evt)
        {
            var locator = new UserLocator.PlatformAccount(evt.Source.Platform, evt.Data.UserId);
            var result = await userService.ResolveAsync(locator);
            switch (result)
            {
                case UserResolutionResult.Resolved resolved:
                    await userService.TryDowngradeRoleAsync(resolved.HellbotUserId, Role.Premium);
                    logger.LogInformation(
                        "User unmodded: EventId={EventId} HellbotUserId={HellbotUserId} PlatformUserId={PlatformUserId} Source={Source}",
                        evt.Id,
                        resolved.HellbotUserId,
                        evt.Data.UserId,
                        evt.Source);
                    break;
                case UserResolutionResult.AmbiguousUsername ambiguous:
                    logger.LogWarning(
                        "User unmodded event {EventId}: unexpected ambiguous resolution for platform account locator; candidates={CandidateHellbotUserIds}",
                        evt.Id,
                        ambiguous.CandidateHellbotUserIds);
                    break;
                case UserResolutionResult.NotFound:
                    logger.LogInformation(
                        "User unmodded: no Hellbot user for {Platform} account {PlatformUserId}; EventId={EventId}",
                        evt.Source.Platform,
                        evt.Data.UserId,
                        evt.Id);
                    break;
            }
        }
    }
}
