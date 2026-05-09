using Hellbot.Core.Users;
using Hellbot.Service.Data;
using Hellbot.Service.Data.Tables.Users;

namespace Hellbot.Service.Users
{
    public class UserService(
        IDbContext db,
        UserTable users,
        UserIdentitiesTable identities,
        UserCache cache,
        ILogger<UserService> logger) : IUserService
    {
        public async Task<User> GetOrCreateUser(UserIdentity identity)
        {
            if (cache.TryGetUser(identity, out User? cacheUser)) return cacheUser;

            var userId = await identities.GetUserId(identity.Platform, identity.UserId);

            if (userId is not Guid id)
            {
                logger.LogInformation("User not found for {Platform}:{PlatformUserId}. Creating new user.", identity.Platform, identity.UserId);
                return await CreateUser(identity);
            }

            cache.MapIdentity(identity, id);
            if (!cache.TryGetUser(id, out User? existingUser))
            {
                existingUser = await users.Get(id);

                if (existingUser is null)
                {
                    logger.LogError("Data inconsistency: Identity({Platform}:${UserId} exists but user {UserId} is missing. Manual repair required.", identity.Platform, identity.UserId, id);
                    throw new InvalidOperationException($"Missing user for identity {id}");
                }

                cache.SetUser(existingUser);
            }

            return existingUser;
        }

        public async Task UpdateUserRoleAsync(UserIdentity identity, Role targetRole)
        {
            var user = await GetOrCreateUser(identity);
            if (user.Role >= targetRole)
                return;

            var updated = user with { Role = targetRole };
            await users.Update(updated);
            cache.SetUser(updated);
        }

        public async Task<bool> UpdateUserRoleForUserAsync(Guid userId, Role targetRole)
        {
            var user = await users.Get(userId);
            if (user is null)
                return false;

            if (user.Role >= targetRole)
                return true;

            var updated = user with { Role = targetRole };
            await users.Update(updated);
            cache.SetUser(updated);
            return true;
        }

        private async Task<User> CreateUser(UserIdentity identity)
        {
            using var tx = db.Connection.BeginTransaction();
            try
            {
                var user = new User
                {
                    Role = Role.User
                };

                await users.Create(user, tx);
                await identities.Create(user, identity, tx);
                tx.Commit();
                cache.MapIdentity(identity, user);
                return user;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<Guid?> GetUserId(UserIdentity userIdentity)
        {
            return await identities.GetUserId(userIdentity.Platform, userIdentity.UserId);
        }
    }
}
