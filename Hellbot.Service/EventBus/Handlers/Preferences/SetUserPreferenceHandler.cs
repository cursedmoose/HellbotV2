using Hellbot.Core.Events.Preferences;
using Hellbot.Service.Entitlements;

namespace Hellbot.Service.EventBus.Handlers.Preferences;

public sealed class SetUserPreferenceHandler(
    IEntitlementService entitlements,
    ILogger<SetUserPreferenceHandler> logger) : EventHandlerBase<SetUserPreference>
{
    public override async Task Handle(SetUserPreference evt)
    {
        try
        {
            await entitlements.UpsertEquippedPreferenceForIdentityAsync(
                evt.Data.Recipient,
                evt.Data.EntitlementType,
                evt.Data.SelectedEntitlementCatalogId);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(
                ex,
                "SetUserPreference skipped for type={Type} catalog={CatalogId}: {Reason}",
                evt.Data.EntitlementType,
                evt.Data.SelectedEntitlementCatalogId,
                ex.Message);
        }
    }
}
