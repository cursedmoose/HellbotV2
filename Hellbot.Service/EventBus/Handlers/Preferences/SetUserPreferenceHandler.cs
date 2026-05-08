using Hellbot.Core.Events.Preferences;
using Hellbot.Service.Data.Tables.Users;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Preferences;

public sealed class SetUserPreferenceHandler(
    IUserService userService,
    UserPreferencesTable preferencesTable,
    UserCache cache,
    ILogger<SetUserPreferenceHandler> logger) : EventHandlerBase<SetUserPreference>
{
    public override async Task Handle(SetUserPreference evt)
    {
        var user = await userService.GetOrCreateUser(evt.Data.Recipient);
        try
        {
            await preferencesTable.UpsertValidatedSelection(user.Id, evt.Data.EntitlementType, evt.Data.SelectedEntitlementCatalogId);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "SetUserPreference skipped: user={UserId} type={Type} catalog={CatalogId}: {Reason}", user.Id, evt.Data.EntitlementType, evt.Data.SelectedEntitlementCatalogId, ex.Message);
            return;
        }

        cache.InvalidateExperience(user.Id);
    }
}
