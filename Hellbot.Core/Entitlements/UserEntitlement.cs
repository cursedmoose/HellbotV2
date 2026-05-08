namespace Hellbot.Core.Entitlements;

public record UserEntitlement
{
    public required DateTime EarnedAt { get; init; }
    public required EntitlementCatalogItem CatalogItem { get; init; }
}
