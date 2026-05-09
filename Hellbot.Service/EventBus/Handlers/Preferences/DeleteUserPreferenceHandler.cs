using Hellbot.Core.Events.Preferences;
using Hellbot.Service.Entitlements;

namespace Hellbot.Service.EventBus.Handlers.Preferences;

public sealed class DeleteUserPreferenceHandler(IEntitlementService entitlements) : EventHandlerBase<DeleteUserPreference>
{
    public override Task Handle(DeleteUserPreference evt)
    {
        return entitlements.ClearEquippedPreferenceForIdentityAsync(evt.Data.Recipient, evt.Data.EntitlementType);
    }
}
