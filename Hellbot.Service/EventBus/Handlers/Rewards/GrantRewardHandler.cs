using Hellbot.Core.Events.Rewards;
using Hellbot.Service.Entitlements;

namespace Hellbot.Service.EventBus.Handlers.Rewards;

public class GrantRewardHandler(
    IEntitlementService entitlements,
    ILogger<GrantRewardHandler> logger) : EventHandlerBase<GrantReward>
{
    public override async Task Handle(GrantReward evt)
    {
        var catalogItemId = evt.Data.EntitlementCatalogItemId;
        var outcome = await entitlements.TryGrantCatalogRewardAsync(evt.Data.Receiver, catalogItemId);

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

            case GrantCatalogItemOutcome.UserMissing:
                logger.LogWarning(
                    "Could not grant catalog item {CatalogItemId} to user={User} as they did not exist!",
                    catalogItemId,
                    evt.Data.Receiver);
                return;

            case GrantCatalogItemOutcome.Duplicate:
                logger.LogWarning(
                    "Duplicate grant skipped for user identity and catalog item {CatalogItemId}.",
                    catalogItemId);
                return;
        }
    }
}
