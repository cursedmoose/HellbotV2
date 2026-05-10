using Hellbot.Core.Entitlements;
using Hellbot.Core.Events;

namespace Hellbot.Core.Events.Preferences;

public record DeleteUserPreferencePayload
{
    public EntitlementType EntitlementType { get; init; }
}

public record DeleteUserPreference : HellbotEvent<DeleteUserPreferencePayload>;
