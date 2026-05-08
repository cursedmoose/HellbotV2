using Dapper;
using Hellbot.Core.Entitlements;
using Hellbot.Service.Data;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Hellbot.Service.Data.Tables.Users;

public class UserEntitlementsTable(IDbContext db)
{
    public enum GrantEntitlementResult
    {
        Granted,
        Duplicate,
    }

    private sealed class EntitlementJoinRow
    {
        public DateTime EarnedAt { get; set; }
        public Guid Id { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string EntitlementId { get; set; } = "";
        public bool IsActive { get; set; }
    }

    public async Task<GrantEntitlementResult> Grant(
        Guid userId,
        Guid entitlementCatalogItemId,
        IDbTransaction? tx = null)
    {
        try
        {
            await db.Connection.ExecuteAsync(
                """
                INSERT INTO user_entitlements (user_id, entitlement_catalog_id, earned_at)
                VALUES (@UserId, @CatalogItemId, @EarnedAt)
                """,
                new
                {
                    UserId = userId,
                    CatalogItemId = entitlementCatalogItemId,
                    EarnedAt = DateTime.UtcNow,
                },
                transaction: tx);
            return GrantEntitlementResult.Granted;
        }
        catch (SqliteException ex) when (SqliteErrors.IsConstraintViolation(ex))
        {
            return GrantEntitlementResult.Duplicate;
        }
    }

    public async Task<IReadOnlyList<UserEntitlement>> GetAll(Guid userId)
    {
        var rows = await db.Connection.QueryAsync<EntitlementJoinRow>(
            """
            SELECT
                ue.earned_at AS EarnedAt,
                c.id AS Id,
                c.entitlement_type AS EntitlementType,
                c.entitlement_id AS EntitlementId,
                c.is_active AS IsActive
            FROM user_entitlements ue
            INNER JOIN entitlement_catalog c ON c.id = ue.entitlement_catalog_id
            WHERE ue.user_id = @UserId
            ORDER BY c.entitlement_type, c.entitlement_id
            """,
            new { UserId = userId });

        return rows
            .Select(r => new UserEntitlement
            {
                EarnedAt = r.EarnedAt,
                CatalogItem = new EntitlementCatalogItem
                {
                    Id = r.Id,
                    EntitlementType = r.EntitlementType,
                    EntitlementId = r.EntitlementId,
                    IsActive = r.IsActive,
                },
            })
            .ToList();
    }
}
