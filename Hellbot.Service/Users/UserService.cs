using Hellbot.Core.Events;
using Hellbot.Core.Users;
using Hellbot.Service.Users.Identity;
using Hellbot.Service.Data;
using Hellbot.Service.Data.Tables.Users;

namespace Hellbot.Service.Users;

public sealed class UserService(
    IDbContext db,
    UserTable users,
    UserIdentitiesTable identities,
    UserCache cache,
    ILogger<UserService> logger) : IUserService
{
    private const int UsernameAmbiguityProbeLimit = 3;

    public Task<User?> GetAsync(Guid hellbotUserId, CancellationToken cancellationToken = default) =>
        users.Get(hellbotUserId);

    public async Task<UserResolutionResult> ResolveAsync(UserLocator locator, CancellationToken cancellationToken = default)
    {
        switch (locator)
        {
            case UserLocator.HellbotUser(Guid id):
            {
                var row = await users.Get(id);
                return row is null ? new UserResolutionResult.NotFound() : new UserResolutionResult.Resolved(id);
            }
            case UserLocator.PlatformAccount(PlatformSource platform, string platformAccountId):
            {
                var id = await identities.GetUserId(platform, platformAccountId);
                return id is null ? new UserResolutionResult.NotFound() : new UserResolutionResult.Resolved(id.Value);
            }
            case UserLocator.PlatformUsername(PlatformSource platform, string username):
            {
                var ids = await identities.GetHellbotUserIdsByUsernameAsync(platform, username, UsernameAmbiguityProbeLimit);
                return ids.Count switch
                {
                    0 => new UserResolutionResult.NotFound(),
                    1 => new UserResolutionResult.Resolved(ids[0]),
                    _ => new UserResolutionResult.AmbiguousUsername(ids),
                };
            }
            default:
                throw new InvalidOperationException($"Unhandled {nameof(UserLocator)}: {locator.GetType().Name}");
        }
    }

    public async Task<User> EnsureUserAsync(UserIdentity snapshot, CancellationToken cancellationToken = default)
    {
        if (cache.TryGetUser(snapshot, out User? cached))
            return cached;

        var userIdFromDb = await identities.GetUserId(snapshot.Platform, snapshot.UserId);

        if (userIdFromDb is not Guid idFromDb)
        {
            logger.LogInformation("User not found for {Platform}:{PlatformUserId}. Creating new user.",
                snapshot.Platform, snapshot.UserId);
            return await CreateUser(snapshot);
        }

        cache.MapHellbotUserId(snapshot, idFromDb);
        if (!cache.TryGetUser(idFromDb, out User? existingUser))
        {
            existingUser = await users.Get(idFromDb);

            if (existingUser is null)
            {
                logger.LogError(
                    "Data inconsistency: Identity({Platform}:{PlatformUserId} exists but Hellbot user {UserId} is missing. Manual repair required.",
                    snapshot.Platform, snapshot.UserId, idFromDb);
                throw new InvalidOperationException($"Missing user for identity {idFromDb}");
            }

            cache.SetUser(existingUser);
        }

        return existingUser;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default) =>
        UpdateUserAndInvalidateCache(user);

    public async Task<bool> TryUpgradeRoleAsync(UserLocator locator, Role targetRole, CancellationToken cancellationToken = default)
    {
        var resolved = await ResolveAsync(locator, cancellationToken);

        if (resolved is not UserResolutionResult.Resolved r)
            return false;

        var hellbotUserId = r.HellbotUserId;

        var user = await users.Get(hellbotUserId);
        if (user is null)
            return false;

        if (user.Role >= targetRole)
            return true;

        var upgraded = user with { Role = targetRole };
        await UpdateUserAndInvalidateCache(upgraded);
        return true;
    }

    private async Task<User> CreateUser(UserIdentity snapshot)
    {
        using var tx = db.Connection.BeginTransaction();
        try
        {
            var user = new User
            {
                Role = Role.User,
            };

            await users.Create(user, tx);
            await identities.Create(user, snapshot, tx);
            tx.Commit();
            cache.MapHellbotUserId(snapshot, user);
            return user;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    private async Task UpdateUserAndInvalidateCache(User user)
    {
        await users.Update(user);
        cache.SetUser(user);
    }
}
