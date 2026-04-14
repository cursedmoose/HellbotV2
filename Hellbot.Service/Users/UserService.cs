using Hellbot.Core.Users;
using Hellbot.Service.Data;
using Hellbot.Service.Data.Tables.Users;

namespace Hellbot.Service.Users
{
    public class UserService(
        IDbContext db,
        UserTable users,
        UserIdentitiesTable userIdentities,
        ILogger<UserService> logger) : IUserService
    {
        public async Task<User> GetOrCreateUser(UserIdentity identity)
        {
            var userId = await userIdentities.GetUserId(identity.Platform, identity.UserId);
            if (userId is Guid id)
            {
                var existingUser = await users.Get(id);
                if (existingUser is not null)
                {
                    return existingUser;
                }
                else
                {
                    logger.LogError("Data inconsistency: Identity({Platform}:${UserId} exists but user {UserId} is missing. Manual repair required.", identity.Platform, identity.UserId, id);
                    throw new InvalidOperationException($"Missing user for identity {id}");
                }
            }

            logger.LogInformation("User not found for {Platform}:{PlatformUserId}. Creating new user.", identity.Platform, identity.UserId);

            using var tx = db.Connection.BeginTransaction();
            try
            {
                var user = new User
                {
                    Role = Core.Commands.Role.User
                };

                await users.Create(user, tx);
                await userIdentities.Create(user, identity, tx);
                tx.Commit();
                return user;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
