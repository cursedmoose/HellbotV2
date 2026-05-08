using Dapper;
using Hellbot.Core.Entitlements;
using Hellbot.Service.Data;
using System.Collections.ObjectModel;
using System.Data;

namespace Hellbot.Service.Data.Tables.Users;

public class UserPreferencesTable(IDbContext db)
{
    /// <summary>Valid selections: user owns the catalog item, type matches preference slot, catalog row is active.</summary>
    public async Task UpsertValidatedSelection(
        Guid userId,
        EntitlementType entitlementType,
        Guid selectedCatalogItemId,
        IDbTransaction? tx = null)
    {
        var typeString = entitlementType.ToString();
        var ownsRaw = await db.Connection.ExecuteScalarAsync<object>(
            """
            SELECT COUNT(1)
            FROM user_entitlements ue
            INNER JOIN entitlement_catalog c ON c.id = ue.entitlement_catalog_id
            WHERE ue.user_id = @UserId
              AND c.id = @CatalogId
              AND c.is_active = 1
              AND c.entitlement_type = @EntitlementType
            """,
            new { UserId = userId, CatalogId = selectedCatalogItemId, EntitlementType = typeString },
            transaction: tx);

        var owns = Convert.ToInt64(ownsRaw ?? 0L);

        if (owns == 0)
            throw new InvalidOperationException(
                $"User {userId} cannot equip catalog item {selectedCatalogItemId}: not granted, missing, inactive, or type mismatch.");

        await db.Connection.ExecuteAsync(
            """
            INSERT INTO user_preferences (user_id, entitlement_type, selected_entitlement_catalog_id)
            VALUES (@UserId, @EntitlementType, @CatalogId)
            ON CONFLICT(user_id, entitlement_type)
            DO UPDATE SET selected_entitlement_catalog_id = excluded.selected_entitlement_catalog_id
            """,
            new { UserId = userId, EntitlementType = typeString, CatalogId = selectedCatalogItemId },
            transaction: tx);
    }

    public async Task<int> DeleteSelection(Guid userId, EntitlementType entitlementType, IDbTransaction? tx = null)
    {
        return await db.Connection.ExecuteAsync(
            """
            DELETE FROM user_preferences
            WHERE user_id = @UserId AND entitlement_type = @EntitlementType
            """,
            new { UserId = userId, EntitlementType = entitlementType.ToString() },
            transaction: tx);
    }

    public async Task<UserExperienceSnapshot> ResolveExperienceAsync(Guid userId, IDbTransaction? tx = null)
    {
        var rows = await db.Connection.QueryAsync<EntitlementCatalogItem>(
            """
            SELECT DISTINCT
                c.id AS Id,
                c.entitlement_type AS EntitlementType,
                c.entitlement_id AS EntitlementId,
                c.is_active AS IsActive
            FROM user_preferences pref
            INNER JOIN user_entitlements ue
                ON ue.user_id = pref.user_id
                AND ue.entitlement_catalog_id = pref.selected_entitlement_catalog_id
            INNER JOIN entitlement_catalog c ON c.id = pref.selected_entitlement_catalog_id
            WHERE pref.user_id = @UserId AND c.is_active = 1
            """,
            new { UserId = userId },
            transaction: tx);

        Dictionary<EntitlementType, EntitlementCatalogItem>? map = null;
        foreach (var row in rows)
        {
            map ??= new Dictionary<EntitlementType, EntitlementCatalogItem>();
            map[row.EntitlementType] = row;
        }

        if (map is null || map.Count == 0)
            return UserExperienceSnapshot.Empty;

        return new UserExperienceSnapshot { Selections = new ReadOnlyDictionary<EntitlementType, EntitlementCatalogItem>(map) };
    }
}
