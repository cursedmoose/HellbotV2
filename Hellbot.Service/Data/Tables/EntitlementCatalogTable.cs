using Dapper;
using Hellbot.Core.Entitlements;
using System.Data;

namespace Hellbot.Service.Data.Tables;

public class EntitlementCatalogTable(IDbContext db)
{
    public async Task Insert(EntitlementCatalogItem item, IDbTransaction? tx = null)
    {
        await db.Connection.ExecuteAsync(
            """
            INSERT INTO entitlement_catalog (id, entitlement_type, entitlement_id, is_active)
            VALUES (@Id, @EntitlementType, @EntitlementId, @IsActive)
            """,
            new
            {
                item.Id,
                EntitlementType = item.EntitlementType.ToString(),
                item.EntitlementId,
                item.IsActive,
            },
            transaction: tx);
    }

    public async Task<EntitlementCatalogItem?> GetById(Guid id)
    {
        return await db.Connection.QuerySingleOrDefaultAsync<EntitlementCatalogItem>(
            """
            SELECT
                id AS Id,
                entitlement_type AS EntitlementType,
                entitlement_id AS EntitlementId,
                is_active AS IsActive
            FROM entitlement_catalog
            WHERE id = @Id
            """,
            new { Id = id });
    }

    public async Task<IReadOnlyList<EntitlementCatalogItem>> GetByType(
        EntitlementType entitlementType,
        IDbTransaction? tx = null)
    {
        var typeString = entitlementType.ToString();
        var rows = await db.Connection.QueryAsync<EntitlementCatalogItem>(
            """
            SELECT
                id AS Id,
                entitlement_type AS EntitlementType,
                entitlement_id AS EntitlementId,
                is_active AS IsActive
            FROM entitlement_catalog
            WHERE entitlement_type = @EntitlementType
            ORDER BY entitlement_id
            """,
            new { EntitlementType = typeString },
            transaction: tx);
        return rows.AsList();
    }

    public async Task<int> SetIsActive(Guid id, bool isActive, IDbTransaction? tx = null)
    {
        return await db.Connection.ExecuteAsync(
            """
            UPDATE entitlement_catalog
            SET is_active = @IsActive
            WHERE id = @Id
            """,
            new { Id = id, IsActive = isActive },
            transaction: tx);
    }
}
