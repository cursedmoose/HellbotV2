using Hellbot.Core.Users;

namespace Hellbot.Service.Users
{
    public interface IUserService
    {
        public Task<User> GetOrCreateUser(UserIdentity identity);
        public Task<UserCustomizationSet> GetUserCustomizations(Guid Id);
    }
}
