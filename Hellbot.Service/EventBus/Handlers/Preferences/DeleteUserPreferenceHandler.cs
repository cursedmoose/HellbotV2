using Hellbot.Core.Events.Preferences;
using Hellbot.Service.Data.Tables.Users;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Preferences;

public sealed class DeleteUserPreferenceHandler(
    IUserService userService,
    UserPreferencesTable preferencesTable,
    UserCache cache) : EventHandlerBase<DeleteUserPreference>
{
    public override async Task Handle(DeleteUserPreference evt)
    {
        var user = await userService.GetOrCreateUser(evt.Data.Recipient);
        await preferencesTable.DeleteSelection(user.Id, evt.Data.EntitlementType);
        cache.InvalidateExperience(user.Id);
    }
}
