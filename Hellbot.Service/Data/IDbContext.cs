using System.Data;

namespace Hellbot.Service.Data
{
    public interface IDbContext
    {
        IDbConnection Connection { get; }
        IDbTransaction BeginTransaction();
    }
}
