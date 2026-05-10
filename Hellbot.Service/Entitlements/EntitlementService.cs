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
        var preferenceSnapshot = await preferencesTable.ResolvePreferencesAsync(user.Id);
        return new UserCapabilitySnapshot(user.Id, granted, preferenceSnapshot);
    }

    public async Task<UserPreferenceSnapshot> GetOrLoadPreferencesAsync(Guid userId)
    {
        if (cache.TryGetPreferences(userId, out var cached))
            return cached;

        var resolved = await preferencesTable.ResolvePreferencesAsync(userId);
        cache.SetPreferences(userId, resolved);
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

    public async Task UpsertEquippedPreferenceAsync(
        Guid hellbotUserId,
        EntitlementType entitlementType,
        Guid selectedCatalogItemId)
    {
        await preferencesTable.UpsertValidatedSelection(hellbotUserId, entitlementType, selectedCatalogItemId);
        cache.InvalidatePreferences(hellbotUserId);
    }

    public async Task ClearEquippedPreferenceAsync(Guid hellbotUserId, EntitlementType entitlementType)
    {
        await preferencesTable.DeleteSelection(hellbotUserId, entitlementType);
        cache.InvalidatePreferences(hellbotUserId);
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
            cache.InvalidatePreferences(user.Id);
            return GrantCatalogItemOutcome.Granted;
        }

        return GrantCatalogItemOutcome.Duplicate;
    }
}
