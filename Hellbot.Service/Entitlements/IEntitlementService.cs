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
    UserMissing,
    Granted,
    Duplicate,
}

public sealed record UserCapabilitySnapshot(
    Guid UserId,
    IReadOnlyList<UserEntitlement> Entitlements,
    UserExperienceSnapshot Experience);

public interface IEntitlementService
{
    Task<UserCapabilitySnapshot> GetCapabilitiesAsync(UserIdentity identity);

    Task<UserExperienceSnapshot> GetOrLoadExperienceSnapshotAsync(Guid userId);

    Task<CreateCatalogItemResult> TryCreateCatalogItemAsync(EntitlementCatalogItem item);

    Task<IReadOnlyList<EntitlementCatalogItem>> GetCatalogByTypeAsync(EntitlementType entitlementType);

    Task<EntitlementCatalogItem?> GetCatalogByIdAsync(Guid id);

    Task<int> SetCatalogItemActiveAsync(Guid id, bool isActive);

    Task UpsertEquippedPreferenceForIdentityAsync(
        UserIdentity recipient,
        EntitlementType entitlementType,
        Guid selectedCatalogItemId);

    Task ClearEquippedPreferenceForIdentityAsync(UserIdentity recipient, EntitlementType entitlementType);

    Task<GrantCatalogItemOutcome> TryGrantCatalogEntitlementAsync(
        UserIdentity recipient,
        Guid entitlementCatalogItemId);
}
