using Hellbot.Core.Events;
using Hellbot.Core.Users;

namespace Hellbot.Service.Users
{
    public interface IUserResolver
    {
        Task<User> Resolve(string UserId, PlatformSource platform);
    }
}
