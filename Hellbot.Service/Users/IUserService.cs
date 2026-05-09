using Hellbot.Core.Users;

namespace Hellbot.Service.Users
{
    public interface IUserService
    {
        public Task<User> GetOrCreateUser(UserIdentity identity);
        public Task UpdateUserRoleAsync(UserIdentity identity, Role targetRole);
        /// <summary>Upgrade role by internal <c>users.id</c>. Returns false if no row exists.</summary>
        public Task<bool> UpdateUserRoleForUserAsync(Guid userId, Role targetRole);
        public Task<Guid?> GetUserId(UserIdentity identity);
    }
}
