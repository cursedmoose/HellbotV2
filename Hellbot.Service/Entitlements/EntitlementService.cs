using Hellbot.Core.Entitlements;
using Hellbot.Core.Users;
using Hellbot.Service.Data.Tables;
using Hellbot.Service.Data.Tables.Users;
using Hellbot.Service.Users;

namespace Hellbot.Service.Entitlements;

public sealed class EntitlementService(
    IUserService userService,
    UserCache cache,
    EntitlementCatalogTable catalog,
    UserEntitlementsTable userEntitlements,
    UserPreferencesTable preferencesTable) : IEntitlementService
{
    public async Task<UserCapabilitySnapshot> GetCapabilitiesAsync(UserIdentity identity)
    {
        var user = await userService.GetOrCreateUserAsync(identity);
        var granted = await userEntitlements.GetAll(user.Id);
        var experience = await preferencesTable.ResolveExperienceAsync(user.Id);
        return new UserCapabilitySnapshot(user.Id, granted, experience);
    }

    public async Task<UserExperienceSnapshot> GetOrLoadExperienceSnapshotAsync(Guid userId)
    {
        if (cache.TryGetExperience(userId, out var cached))
            return cached;

        var resolved = await preferencesTable.ResolveExperienceAsync(userId);
        cache.SetExperience(userId, resolved);
        return resolved;
    }

    public async Task<CreateCatalogItemResult> TryCreateCatalogItemAsync(EntitlementCatalogItem item)
    {
        var inserted = await catalog.TryInsert(item);
        return inserted == EntitlementCatalogTable.CatalogInsertResult.Created
            ? CreateCatalogItemResult.Created
            : CreateCatalogItemResult.DuplicateKey;
    }

    public Task<IReadOnlyList<EntitlementCatalogItem>> GetCatalogByTypeAsync(EntitlementType entitlementType)
        => catalog.GetByType(entitlementType);

    public Task<EntitlementCatalogItem?> GetCatalogByIdAsync(Guid id)
        => catalog.GetById(id);

    public Task<int> SetCatalogItemActiveAsync(Guid id, bool isActive)
        => catalog.SetIsActive(id, isActive);

    public async Task UpsertEquippedPreferenceForIdentityAsync(
        UserIdentity recipient,
        EntitlementType entitlementType,
        Guid selectedCatalogItemId)
    {
        var user = await userService.GetOrCreateUserAsync(recipient);
        await preferencesTable.UpsertValidatedSelection(user.Id, entitlementType, selectedCatalogItemId);
        cache.InvalidateExperience(user.Id);
    }

    public async Task ClearEquippedPreferenceForIdentityAsync(UserIdentity recipient, EntitlementType entitlementType)
    {
        var user = await userService.GetOrCreateUserAsync(recipient);
        await preferencesTable.DeleteSelection(user.Id, entitlementType);
        cache.InvalidateExperience(user.Id);
    }

    public async Task<GrantCatalogItemOutcome> TryGrantCatalogEntitlementAsync(
        UserIdentity recipient,
        Guid entitlementCatalogItemId)
    {
        var catalogItem = await catalog.GetById(entitlementCatalogItemId);
        if (catalogItem is null)
            return GrantCatalogItemOutcome.CatalogItemMissing;

        if (!catalogItem.IsActive)
            return GrantCatalogItemOutcome.CatalogItemInactive;

        var user = await userService.GetOrCreateUserAsync(recipient);

        var grantResult = await userEntitlements.Grant(user.Id, entitlementCatalogItemId);
        if (grantResult == UserEntitlementsTable.GrantEntitlementResult.Granted)
        {
            cache.InvalidateExperience(user.Id);
            return GrantCatalogItemOutcome.Granted;
        }

        return GrantCatalogItemOutcome.Duplicate;
    }
}
