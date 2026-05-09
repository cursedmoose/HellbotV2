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
}
