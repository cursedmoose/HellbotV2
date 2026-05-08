using Hellbot.Core.Entitlements;
using Hellbot.Core.Events;
using Hellbot.Core.Users;

namespace Hellbot.Core.Events.Preferences;

public record DeleteUserPreferencePayload
{
    public required UserIdentity Recipient { get; init; }
    public EntitlementType EntitlementType { get; init; }
}

public record DeleteUserPreference : HellbotEvent<DeleteUserPreferencePayload>;
