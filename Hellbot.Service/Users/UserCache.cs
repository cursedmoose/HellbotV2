using Hellbot.Core.Users;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Hellbot.Service.Users
{
    public class UserCache
    {
        private readonly ConcurrentDictionary<UserIdentity, Guid> _identityToUserId = new();
        private readonly ConcurrentDictionary<Guid, User> _users = new();
        private readonly ConcurrentDictionary<Guid, UserCustomizationSet> _customizations = new();

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

        public void SetCustomizations(Guid userId, UserCustomizationSet set)
        {
            _customizations[userId] = set;
        }

        public bool TryGetCustomizations(Guid userId, [MaybeNullWhen(false)] out UserCustomizationSet set)
        {
            return _customizations.TryGetValue(userId, out set);
        }

        public bool TryGetCustomizations(UserIdentity identity, [MaybeNullWhen(false)] out UserCustomizationSet set)
        {
            if (TryGetUserId(identity, out var userId))
            {
                return _customizations.TryGetValue(userId, out set);
            }
            set = null;
            return false;
        }

        public void InvalidateUser(Guid userId)
        {
            _users.TryRemove(userId, out _);
            _customizations.TryRemove(userId, out _);

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
