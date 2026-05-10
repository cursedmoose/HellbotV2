using Hellbot.Core.Entitlements;
using Hellbot.Core.Users;

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
    Task<UserCapabilitySnapshot> GetCapabilitiesAsync(UserIdentity identity);

    Task<UserPreferenceSnapshot> GetOrLoadPreferencesAsync(Guid userId);

    Task<CreateCatalogItemResult> TryCreateCatalogItemAsync(EntitlementCatalogItem item);

    Task<IReadOnlyList<EntitlementCatalogItem>> GetCatalogByTypeAsync(EntitlementType entitlementType);

    Task<EntitlementCatalogItem?> GetCatalogByIdAsync(Guid id);

    Task<int> SetCatalogItemActiveAsync(Guid id, bool isActive);

    Task UpsertEquippedPreferenceAsync(Guid hellbotUserId, EntitlementType entitlementType, Guid selectedCatalogItemId);

    Task ClearEquippedPreferenceAsync(Guid hellbotUserId, EntitlementType entitlementType);

    Task<GrantCatalogItemOutcome> TryGrantCatalogEntitlementAsync(
        UserIdentity recipient,
        Guid entitlementCatalogItemId);
}
