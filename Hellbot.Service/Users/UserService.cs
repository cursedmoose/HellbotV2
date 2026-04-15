using Hellbot.Core.TTS;
using Hellbot.Core.Users;
using Hellbot.Service.Data;
using Hellbot.Service.Data.Tables.Users;
using System.Text.Json;

namespace Hellbot.Service.Users
{
    public class UserService(
        IDbContext db,
        UserTable users,
        UserIdentitiesTable identities,
        UserCustomizationsTable customizations,
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

        private async Task<User> CreateUser(UserIdentity identity)
        {
            using var tx = db.Connection.BeginTransaction();
            try
            {
                var user = new User
                {
                    Role = Core.Commands.Role.User
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

        public async Task<UserCustomizationSet> GetUserCustomizations(Guid userId)
        {
            if (cache.TryGetCustomizations(userId, out UserCustomizationSet? set)) return set;

            var userCustomizations = await customizations.GetAll(userId);
            UserCustomizationSet settings = new();

            foreach (var customization in userCustomizations)
            {
                switch (customization.Type)
                {
                    case CustomizationType.VoiceId:
                        settings.VoiceId = customization.Value;
                        break;
                    case CustomizationType.VoiceSettings:
                        settings.VoiceSettings = JsonSerializer.Deserialize<VoiceSettings>(customization.Value);
                        break;
                    case CustomizationType.SceneId:
                        settings.SceneId = customization.Value;
                        break;
                }
            }

            cache.SetCustomizations(userId, settings);
            return settings;
        }
    }
}
