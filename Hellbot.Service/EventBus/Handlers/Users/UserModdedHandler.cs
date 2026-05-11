using Hellbot.Core.Events;
using Hellbot.Core.Events.Users;
using Hellbot.Core.Users;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Users;
using Hellbot.Service.Users.Identity;

namespace Hellbot.Service.EventBus.Handlers.Users
{
    public class UserModdedHandler(IUserService userService, ILogger<UserModdedHandler> logger) : EventHandlerBase<UserModded>
    {
        public override async Task Handle(UserModded evt)
        {
            var locator = new UserLocator.PlatformAccount(evt.Source.Platform, evt.Data.UserId);
            var result = await userService.ResolveAsync(locator);
            switch (result)
            {
                case UserResolutionResult.Resolved resolved:
                    await userService.TryUpgradeRoleAsync(resolved.HellbotUserId, Role.Moderator);
                    logger.LogInformation(
                        "User modded: EventId={EventId} HellbotUserId={HellbotUserId} PlatformUserId={PlatformUserId} Source={Source}",
                        evt.Id,
                        resolved.HellbotUserId,
                        evt.Data.UserId,
                        evt.Source);
                    break;
                case UserResolutionResult.AmbiguousUsername ambiguous:
                    logger.LogWarning(
                        "User modded event {EventId}: unexpected ambiguous resolution for platform account locator; candidates={CandidateHellbotUserIds}",
                        evt.Id,
                        ambiguous.CandidateHellbotUserIds);
                    break;
                case UserResolutionResult.NotFound:
                    logger.LogInformation(
                        "User modded: no Hellbot user for {Platform} account {PlatformUserId}; EventId={EventId}",
                        evt.Source.Platform,
                        evt.Data.UserId,
                        evt.Id);
                    break;
            }
        }
    }
}
