using Hellbot.Core.Users;
using Hellbot.Service.Users.Identity;

namespace Hellbot.Service.Users;

public interface IUserService
{
    Task<User?> GetAsync(Guid hellbotUserId, CancellationToken cancellationToken = default);

    Task<UserResolutionResult> ResolveAsync(UserLocator locator, CancellationToken cancellationToken = default);

    /// <summary>Returns the Hellbot user for this platform snapshot, or creates the user and identity row when missing. Lookup key is <see cref="UserIdentity.Platform"/> + immutable <see cref="UserIdentity.UserId"/> (platform account id).</summary>
    Task<User> GetOrCreateUserAsync(UserIdentity snapshot, CancellationToken cancellationToken = default);

    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    Task<bool> TryUpgradeRoleAsync(Guid userId, Role targetRole, CancellationToken cancellationToken = default);

    Task<bool> TryDowngradeRoleAsync(Guid userId, Role targetRole, CancellationToken cancellationToken = default);
}
