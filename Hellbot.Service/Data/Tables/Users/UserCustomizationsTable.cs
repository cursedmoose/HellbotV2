using Dapper;
using Hellbot.Core.Users;
using System.Data;

namespace Hellbot.Service.Data.Tables.Users
{
    public class UserCustomizationsTable(IDbContext db)
    {
        public async Task Upsert(
            Guid userId,
            UserCustomization customization,
            IDbTransaction? tx = null)
        {
            await db.Connection.ExecuteAsync(@"
            INSERT INTO user_customizations (user_id, type, value, updated_at)
            VALUES (@UserId, @Type, @Value, @UpdatedAt)
            ON CONFLICT(user_id, type) DO UPDATE SET
                value = excluded.value,
                updated_at = excluded.updated_at
        ",
            new
            {
                UserId = userId,
                Type = customization.Type,
                Value = customization.Value,
                UpdatedAt = DateTimeOffset.UtcNow
            },
            transaction: tx);
        }

        public async Task<IReadOnlyList<UserCustomization>> GetAll(Guid userId)
        {
            return (await db.Connection.QueryAsync<UserCustomization>(@"
            SELECT
                user_id AS UserId,
                type,
                value
            FROM user_customizations
            WHERE user_id = @UserId
        ",
            new { UserId = userId }))
            .AsList();
        }
    }
}
