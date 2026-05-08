using Hellbot.Core.Entitlements;
using Hellbot.Core.Users;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Hellbot.Service.Users
{
    public class UserCache
    {
        private readonly ConcurrentDictionary<UserIdentity, Guid> _identityToUserId = new();
        private readonly ConcurrentDictionary<Guid, User> _users = new();
        private readonly ConcurrentDictionary<Guid, UserExperienceSnapshot> _experienceSnapshots = new();

        public void MapIdentity(UserIdentity identity, Guid userId)
        {
            _identityToUserId[identity] = userId;
        }

        public void MapIdentity(UserIdentity identity, User user)
        {
            MapIdentity(identity, user.Id);
            SetUser(user);
        }

        public bool TryGetUserId(UserIdentity identity, out Guid userId)
        {
            return _identityToUserId.TryGetValue(identity, out userId);
        }

        public void SetUser(User user)
        {
            _users[user.Id] = user;
        }

        public bool TryGetUser(UserIdentity identity, [MaybeNullWhen(false)] out User user)
        {
            if (TryGetUserId(identity, out var userId))
            {
                return _users.TryGetValue(userId, out user);
            }
            user = null;
            return false;
        }

        public bool TryGetUser(Guid userId, [MaybeNullWhen(false)] out User user)
        {
            return _users.TryGetValue(userId, out user);
        }

        public bool TryGetExperience(Guid userId, [MaybeNullWhen(false)] out UserExperienceSnapshot snapshot)
        {
            return _experienceSnapshots.TryGetValue(userId, out snapshot);
        }

        public void SetExperience(Guid userId, UserExperienceSnapshot snapshot)
        {
            _experienceSnapshots[userId] = snapshot;
        }

        /// <summary>Call when preferences change or entitlement grants revoke equipment validity.</summary>
        public void InvalidateExperience(Guid userId)
        {
            _experienceSnapshots.TryRemove(userId, out _);
        }

        public void InvalidateUser(Guid userId)
        {
            _users.TryRemove(userId, out _);
            _experienceSnapshots.TryRemove(userId, out _);

            var identitiesToRemove = _identityToUserId
                .Where(kvp => kvp.Value == userId)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var id in identitiesToRemove)
            {
                _identityToUserId.TryRemove(id, out _);
            }
        }
    }
}
