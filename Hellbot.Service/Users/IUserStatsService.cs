using Hellbot.Core.Users;

namespace Hellbot.Service.Users
{
    public interface IUserStatsService
    {
        Task RecordChat(User user);
    }
}
