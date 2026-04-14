using Dapper;
using Hellbot.Core.Events;
using Hellbot.Core.Users;
using System.Data;

namespace Hellbot.Service.Data.Tables.Users
{
    public class UserIdentitiesTable(IDbContext db)
    {
        public async Task Create(User user, UserIdentity identity, IDbTransaction? tx = null)
        {
            await db.Connection.ExecuteAsync(@"
            INSERT INTO user_identities 
            (user_id, platform, platform_user_id, platform_user_name, linked_at)
            VALUES 
            (@UserId, @Platform, @PlatformUserId, @PlatformUserName, @LinkedAt)
            ", new
            {
                UserId = user.Id,
                Platform = identity.Platform.ToString(),
                PlatformUserId = identity.UserId,
                PlatformUserName = identity.Username,
                LinkedAt = DateTimeOffset.UtcNow,
            },
            transaction: tx);
        }

        public async Task<Guid?> GetUserId(PlatformSource platform, string userId)
        {
            return await db.Connection.QuerySingleOrDefaultAsync<Guid?>(@"
                SELECT user_id
                FROM user_identities
                WHERE platform = @Platform
                  AND platform_user_id = @PlatformUserId
            ",
            new
            {
                Platform = platform.ToString(),
                PlatformUserId = userId
            });
        }

        public async Task<IReadOnlyList<UserIdentity>> Get(Guid userId)
        {
            return (await db.Connection.QueryAsync<UserIdentity>(@"
                SELECT
                    user_id AS UserId,
                    platform AS Platform,
                    platform_user_id AS PlatformUserId,
                    platform_user_name AS PlatformUserName,
                    linked_at AS LinkedAt
                FROM user_identities
                WHERE user_id = @UserId
            ", new { UserId = userId }))
            .AsList();
        }
    }
}
