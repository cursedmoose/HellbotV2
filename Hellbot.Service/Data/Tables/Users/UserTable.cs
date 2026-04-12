using Dapper;
using Hellbot.Core.TTS;
using Hellbot.Core.Users;

namespace Hellbot.Service.Data.Tables.Users
{
    public class UserTable(IDbConnectionFactory factory)
    {
        public async Task Create(User user)
        {
            using var connection = factory.CreateConnection();

            await connection.ExecuteAsync(@"
            INSERT INTO users (id, twitchId)
            VALUES (@Id, @TwitchId)
            ", new
            {
                user.Id,
                user.TwitchId
            });
        }

        public async Task<User?> GetByTwitchId(string TwitchId)
        {
            using var connection = factory.CreateConnection();
            var user = await connection.QuerySingleOrDefaultAsync<User>(@"
                SELECT id, twitchId
                FROM users
                WHERE twitchId = @TwitchId
            ", new { TwitchId });

            return user;
        }
    }
}
