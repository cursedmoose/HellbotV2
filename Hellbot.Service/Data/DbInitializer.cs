using Hellbot.Service.Config;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Hellbot.Service.Data
{
    public class DbInitializer(IOptions<DbOptions> options)
    {
        private readonly string _connectionString = options.Value.ConnectionString;

        public async Task InitializeAsync()
        {
            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = CreateEventsTable;

            await cmd.ExecuteNonQueryAsync();
        }

        const string CreateEventsTable = @"
            CREATE TABLE IF NOT EXISTS events (
                id TEXT PRIMARY KEY,
                timestamp DATETIME NOT NULL,
                platform TEXT NOT NULL,
                event_type TEXT NOT NULL,
                metadata JSON
            );";
    }
}
