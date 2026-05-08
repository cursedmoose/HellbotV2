namespace Hellbot.Core.Entitlements;

public record EntitlementCatalogItem
{
    public required Guid Id { get; init; }
    public required EntitlementType EntitlementType { get; init; }
    public required string EntitlementId { get; init; }
    public required bool IsActive { get; init; }
}
