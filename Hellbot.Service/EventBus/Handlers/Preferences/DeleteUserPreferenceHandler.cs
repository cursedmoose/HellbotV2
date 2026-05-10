using Hellbot.Core.Events.Preferences;
using Hellbot.Service.Entitlements;

namespace Hellbot.Service.EventBus.Handlers.Preferences;

public sealed class DeleteUserPreferenceHandler(
    IEntitlementService entitlements,
    ILogger<DeleteUserPreferenceHandler> logger) : EventHandlerBase<DeleteUserPreference>
{
    public override async Task Handle(DeleteUserPreference evt)
    {
        if (!evt.Context.TryGetUser(out var user))
        {
            logger.LogWarning("DeleteUserPreference skipped for event={EventId}. No persisted user context.", evt.Id);
            return;
        }

        await entitlements.ClearEquippedPreferenceAsync(user.Id, evt.Data.EntitlementType);
    }
}
