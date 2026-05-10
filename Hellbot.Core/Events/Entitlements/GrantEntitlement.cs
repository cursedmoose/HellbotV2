using Hellbot.Core.Events;

namespace Hellbot.Core.Events.Entitlements;

public record GrantEntitlementPayload
{
    public required Guid UserId { get; init; }
    public required Guid EntitlementCatalogItemId { get; init; }
}

public record GrantEntitlement : HellbotEvent<GrantEntitlementPayload>;
