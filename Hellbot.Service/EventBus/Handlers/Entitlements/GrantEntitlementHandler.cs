using Hellbot.Core.Events.Entitlements;
using Hellbot.Service.Entitlements;

namespace Hellbot.Service.EventBus.Handlers.Entitlements;

public class GrantEntitlementHandler(
    IEntitlementService entitlements,
    ILogger<GrantEntitlementHandler> logger) : EventHandlerBase<GrantEntitlement>
{
    public override async Task Handle(GrantEntitlement evt)
    {
        var catalogItemId = evt.Data.EntitlementCatalogItemId;
        var outcome = await entitlements.TryGrantCatalogEntitlementAsync(evt.Data.Receiver, catalogItemId);

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
                    "Duplicate grant skipped for user identity and catalog item {CatalogItemId}.",
                    catalogItemId);
                return;
        }
    }
}
