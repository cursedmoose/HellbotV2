using Hellbot.Core.Entitlements;
using Hellbot.Core.Events;
using Hellbot.Core.Users;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Hellbot.Service.Users;

public class UserCache
{
    private readonly ConcurrentDictionary<(PlatformSource Platform, string PlatformAccountId), Guid> _platformToHellbotUserId = new();
    private readonly ConcurrentDictionary<Guid, User> _users = new();
    private readonly ConcurrentDictionary<Guid, UserExperienceSnapshot> _experienceSnapshots = new();

    public static (PlatformSource Platform, string PlatformAccountId) PlatformAccountKey(UserIdentity identity) =>
        (identity.Platform, identity.UserId);

    /// <inheritdoc cref="TryGetHellbotUserId(PlatformSource, string, out Guid)"/>
    public bool TryGetHellbotUserId(UserIdentity identity, [MaybeNullWhen(false)] out Guid hellbotUserId) =>
        TryGetHellbotUserId(identity.Platform, identity.UserId, out hellbotUserId);

    public bool TryGetHellbotUserId(PlatformSource platform, string platformAccountId, out Guid hellbotUserId) =>
        _platformToHellbotUserId.TryGetValue((platform, platformAccountId), out hellbotUserId);

    public void MapHellbotUserId(UserIdentity snapshot, Guid hellbotUserId) =>
        _platformToHellbotUserId[PlatformAccountKey(snapshot)] = hellbotUserId;

    public void MapHellbotUserId(UserIdentity snapshot, User user)
    {
        MapHellbotUserId(snapshot, user.Id);
        SetUser(user);
    }

    public void SetUser(User user) =>
        _users[user.Id] = user;

    public bool TryGetUser(UserIdentity identity, [MaybeNullWhen(false)] out User user)
    {
        if (TryGetHellbotUserId(identity, out var hellbotUserId))
            return _users.TryGetValue(hellbotUserId, out user);
        user = null;
        return false;
    }

    public bool TryGetUser(Guid hellbotUserId, [MaybeNullWhen(false)] out User user) =>
        _users.TryGetValue(hellbotUserId, out user);

    public bool TryGetExperience(Guid hellbotUserId, [MaybeNullWhen(false)] out UserExperienceSnapshot snapshot) =>
        _experienceSnapshots.TryGetValue(hellbotUserId, out snapshot);

    public void SetExperience(Guid hellbotUserId, UserExperienceSnapshot snapshot) =>
        _experienceSnapshots[hellbotUserId] = snapshot;

    /// <summary>Call when preferences change or entitlement grants revoke equipment validity.</summary>
    public void InvalidateExperience(Guid hellbotUserId) =>
        _experienceSnapshots.TryRemove(hellbotUserId, out _);

    public void InvalidateUser(Guid hellbotUserId)
    {
        _users.TryRemove(hellbotUserId, out _);
        _experienceSnapshots.TryRemove(hellbotUserId, out _);

        var keys = _platformToHellbotUserId
            .Where(kvp => kvp.Value == hellbotUserId)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keys)
            _platformToHellbotUserId.TryRemove(key, out _);
    }
}
