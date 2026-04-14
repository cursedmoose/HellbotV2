using Dapper;
using Hellbot.Core.Users;
using System.Data;

namespace Hellbot.Service.Data.Tables.Users
{
    public class UserTable(IDbContext db)
    {
        public async Task Create(User user, IDbTransaction? tx = null)
        {
            await db.Connection.ExecuteAsync(@"
            INSERT INTO users (id, status, role, joined_at)
            VALUES (@Id, @Status, @Role, @JoinedAt)
        ",
            new
            {
                Id = user.Id,
                Status = user.Status,
                Role = user.Role.ToString(),
                JoinedAt = user.JoinedAt
            },
            transaction: tx);
        }

        public async Task<User?> Get(Guid id)
        {
            return await db.Connection.QuerySingleOrDefaultAsync<User>(@"
            SELECT
                id,
                status,
                role,
                joined_at AS JoinedAt
            FROM users
            WHERE id = @Id
        ",
            new { Id = id });
        }

        public async Task Update(User user, IDbTransaction? tx = null)
        {
            await db.Connection.ExecuteAsync(@"
            UPDATE users
            SET status = @Status,
                role = @Role,
                joined_at = @JoinedAt
            WHERE id = @Id
        ",
            new
            {
                Id = user.Id,
                Status = user.Status,
                Role = user.Role.ToString(),
                JoinedAt = user.JoinedAt
            },
            transaction: tx);
        }
    }
}
