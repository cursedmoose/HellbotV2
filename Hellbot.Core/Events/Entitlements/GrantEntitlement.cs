using Hellbot.Core.Events;
using Hellbot.Core.Users;

namespace Hellbot.Core.Events.Entitlements;

public record GrantEntitlementPayload
{
    public required UserIdentity Receiver { get; init; }
    public required Guid EntitlementCatalogItemId { get; init; }
}

public record GrantEntitlement : HellbotEvent<GrantEntitlementPayload>;
