using Hellbot.Service.Config;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using System.Data;

namespace Hellbot.Service.Data
{
    public class SqliteConnectionFactory(IOptions<DbOptions> options) : IDbConnectionFactory
    {
        private readonly string _connectionString = options.Value.ConnectionString;

        public IDbConnection CreateConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // One-time pragmas per connection
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "PRAGMA journal_mode=WAL;";
            cmd.ExecuteNonQuery();

            return connection;
        }
    }
}
