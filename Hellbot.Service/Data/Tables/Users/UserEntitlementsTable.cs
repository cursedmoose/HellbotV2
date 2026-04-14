using Dapper;
using Hellbot.Core.Users;
using System.Data;

namespace Hellbot.Service.Data.Tables.Users
{
    public class UserEntitlementsTable(IDbContext db)
    {
        public async Task Create(
            Guid userId,
            UserCustomization customization,
            IDbTransaction? tx = null)
        {
            await db.Connection.ExecuteAsync(@"
            INSERT INTO user_entitlements
                (user_id, type, key, metadata, earned_at)
            VALUES
                (@UserId, @Type, @Key, @Metadata, @EarnedAt)
        ",
            new
            {
                UserId = userId,
                Type = customization.Type.ToString(),
                Value = customization.Value,
                EarnedAt = DateTimeOffset.UtcNow
            },
            transaction: tx);
        }

        public async Task<UserCustomization?> Get(Guid userId, CustomizationType type)
        {
            return await db.Connection.QuerySingleOrDefaultAsync<UserCustomization>(@"
            SELECT
                user_id AS UserId,
                type,
                value
            FROM user_entitlements
            WHERE user_id = @UserId
              AND type = @Type
        ",
            new
            {
                UserId = userId,
                Type = type.ToString(),
            });
        }

        public async Task<IReadOnlyList<UserCustomization>> GetAll(Guid userId)
        {
            return (await db.Connection.QueryAsync<UserCustomization>(@"
            SELECT
                user_id AS UserId,
                type,
                value
            FROM user_entitlements
            WHERE user_id = @UserId
        ",
            new { UserId = userId }))
            .AsList();
        }
    }
}
