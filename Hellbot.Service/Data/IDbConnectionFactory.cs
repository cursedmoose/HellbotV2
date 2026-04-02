using System.Data;

namespace Hellbot.Service.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
