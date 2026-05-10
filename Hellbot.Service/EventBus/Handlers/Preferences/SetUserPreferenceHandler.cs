using Hellbot.Core.Events.Preferences;
using Hellbot.Service.Entitlements;

namespace Hellbot.Service.EventBus.Handlers.Preferences;

public sealed class SetUserPreferenceHandler(
    IEntitlementService entitlements,
    ILogger<SetUserPreferenceHandler> logger) : EventHandlerBase<SetUserPreference>
{
    public override async Task Handle(SetUserPreference evt)
    {
        if (!evt.Context.TryGetPersistedUser(out var user))
        {
            logger.LogWarning("SetUserPreference skipped for event={EventId}. No persisted user context.", evt.Id);
            return;
        }

        try
        {
            await entitlements.UpsertEquippedPreferenceAsync(
                user.Id,
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
