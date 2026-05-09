using Hellbot.Core.Users;
using Hellbot.Service.Users.Identity;

namespace Hellbot.Service.Users;

public interface IUserService
{
    Task<User?> GetAsync(Guid hellbotUserId, CancellationToken cancellationToken = default);

    Task<UserResolutionResult> ResolveAsync(UserLocator locator, CancellationToken cancellationToken = default);

    /// <summary>Load or provision a Hellbot user linked to this platform snapshot (immutable key: platform + snapshot.UserId).</summary>
    Task<User> EnsureUserAsync(UserIdentity snapshot, CancellationToken cancellationToken = default);

    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    Task<bool> TryUpgradeRoleAsync(UserLocator locator, Role targetRole, CancellationToken cancellationToken = default);

    [Obsolete($"{nameof(GetOrCreateUser)} is replaced by {nameof(EnsureUserAsync)}.")]
    Task<User> GetOrCreateUser(UserIdentity identity);

    [Obsolete($"{nameof(UpdateUserRoleAsync)}: use {nameof(EnsureUserAsync)} then {nameof(TryUpgradeRoleAsync)} ({nameof(UserLocator.FromIdentity)}).")]
    Task UpdateUserRoleAsync(UserIdentity identity, Role targetRole);

    [Obsolete($"{nameof(UpdateUserRoleForUserAsync)}: use {nameof(TryUpgradeRoleAsync)} ({nameof(UserLocator)}.{nameof(UserLocator.HellbotUser)}, ...).")]
    Task<bool> UpdateUserRoleForUserAsync(Guid userId, Role targetRole);

    [Obsolete($"{nameof(GetUserId)}: use {nameof(ResolveAsync)} ({nameof(UserLocator.FromIdentity)}, ...).")]
    Task<Guid?> GetUserId(UserIdentity identity);
}
