using Hellbot.Core.Events;
using Hellbot.Core.Users;
using Hellbot.Service.Data.Tables.Users;

namespace Hellbot.Service.Users
{
    public class UserResolver(UserTable users) : IUserResolver
    {
        private Dictionary<UserIdentity, User> _userCache = new();
        public async Task<User> Resolve(string userId, PlatformSource platform)
        {
            var identity = new UserIdentity { Platform = platform, UserId = userId };

            User? user = null;
            switch (platform)
            {
                case PlatformSource.Twitch:
                    user = await users.GetByTwitchId(userId);
                    break;
            }

            if (user == null)
            {
                user = new User { TwitchId = userId };
                await users.Create(user);
            }

            return user;
        }
    }
}
