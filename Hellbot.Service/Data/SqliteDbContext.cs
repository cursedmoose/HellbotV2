using System.Data;

namespace Hellbot.Service.Data
{
    public class SqliteDbContext(IDbConnectionFactory factory) : IDbContext
    {
        public IDbConnection Connection { get; } = factory.CreateConnection();

        public IDbTransaction BeginTransaction()
            => Connection.BeginTransaction();
    }
}
