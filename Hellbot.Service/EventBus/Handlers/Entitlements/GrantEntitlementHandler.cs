using Hellbot.Core.Events.Entitlements;
using Hellbot.Service.Entitlements;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Entitlements;

public class GrantEntitlementHandler(
    IUserService userService,
    IEntitlementService entitlements,
    ILogger<GrantEntitlementHandler> logger) : EventHandlerBase<GrantEntitlement>
{
    public override async Task Handle(GrantEntitlement evt)
    {
        var catalogItemId = evt.Data.EntitlementCatalogItemId;
        var user = await userService.GetAsync(evt.Data.UserId);
        if (user is null)
        {
            logger.LogWarning("Could not grant catalog item {CatalogItemId}: unknown Hellbot user id {HellbotUserId}.", catalogItemId, evt.Data.UserId);
            return;
        }

        var outcome = await entitlements.TryGrantCatalogEntitlementAsync(user.Id, catalogItemId);

        switch (outcome)
        {
            case GrantCatalogItemOutcome.Granted:
                return;

            case GrantCatalogItemOutcome.CatalogItemMissing:
                logger.LogWarning("Could not grant catalog item {CatalogItemId}: not found.", catalogItemId);
                return;

            case GrantCatalogItemOutcome.CatalogItemInactive:
                logger.LogWarning("Could not grant catalog item {CatalogItemId}: inactive in catalog.", catalogItemId);
                return;

            case GrantCatalogItemOutcome.Duplicate:
                logger.LogWarning(
                    "Duplicate grant skipped for Hellbot user {HellbotUserId} and catalog item {CatalogItemId}.",
                    user.Id,
                    catalogItemId);
                return;
        }
    }
}
