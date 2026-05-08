using Hellbot.Core.Entitlements;
using Hellbot.Core.Events;
using Hellbot.Core.Users;

namespace Hellbot.Core.Events.Preferences;

public record SetUserPreferencePayload
{
    public required UserIdentity Recipient { get; init; }
    public EntitlementType EntitlementType { get; init; }
    public required Guid SelectedEntitlementCatalogId { get; init; }
}

public record SetUserPreference : HellbotEvent<SetUserPreferencePayload>;
