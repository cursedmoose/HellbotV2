using Hellbot.Core.Commands;
using Hellbot.Core.Users;

namespace Hellbot.Service.Users
{
    public interface IUserService
    {
        public Task<User> GetOrCreateUser(UserIdentity identity);
        public Task UpdateUserRoleAsync(UserIdentity identity, Role targetRole);
        public Task<UserCustomizationSet> GetUserCustomizations(Guid Id);
        public Task<Guid?> GetUserId(UserIdentity identity);
    }
}
