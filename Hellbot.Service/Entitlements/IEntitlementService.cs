using Hellbot.Core.Entitlements;

namespace Hellbot.Service.Entitlements;

public enum CreateCatalogItemResult
{
    Created,
    DuplicateKey,
}

/// <summary>Deterministic outcome when attempting to grant from catalog identity + ids.</summary>
public enum GrantCatalogItemOutcome
{
    CatalogItemMissing,
    CatalogItemInactive,
    Granted,
    Duplicate,
}

public sealed record UserCapabilitySnapshot(
    Guid UserId,
    IReadOnlyList<UserEntitlement> Entitlements,
    UserPreferenceSnapshot PreferenceSnapshot);

public interface IEntitlementService
{
    Task<UserCapabilitySnapshot> GetCapabilitiesAsync(Guid hellbotUserId);

    Task<UserPreferenceSnapshot> GetOrLoadPreferencesAsync(Guid userId);

    Task<CreateCatalogItemResult> TryCreateCatalogItemAsync(EntitlementCatalogItem item);

    Task<IReadOnlyList<EntitlementCatalogItem>> GetCatalogByTypeAsync(EntitlementType entitlementType);

    Task<EntitlementCatalogItem?> GetCatalogByIdAsync(Guid id);

    Task<int> SetCatalogItemActiveAsync(Guid id, bool isActive);

    Task UpsertEquippedPreferenceAsync(Guid hellbotUserId, EntitlementType entitlementType, Guid selectedCatalogItemId);

    Task ClearEquippedPreferenceAsync(Guid hellbotUserId, EntitlementType entitlementType);

    Task<GrantCatalogItemOutcome> TryGrantCatalogEntitlementAsync(
        Guid hellbotUserId,
        Guid entitlementCatalogItemId);
}
