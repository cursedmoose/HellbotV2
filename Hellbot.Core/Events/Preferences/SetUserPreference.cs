using Hellbot.Core.Entitlements;
using Hellbot.Core.Events;

namespace Hellbot.Core.Events.Preferences;

public record SetUserPreferencePayload
{
    public EntitlementType EntitlementType { get; init; }
    public required Guid SelectedEntitlementCatalogId { get; init; }
}

public record SetUserPreference : HellbotEvent<SetUserPreferencePayload>;
