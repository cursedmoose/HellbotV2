using System.Diagnostics.CodeAnalysis;
using Hellbot.Core.Entitlements;
using Hellbot.Core.Sessions;
using Hellbot.Core.Users;

namespace Hellbot.Core.Events
{
    public record struct EventContext
    {
        public UserContext? User { get; set; }
        public StreamSessionSnapshot? Stream { get; set; }

        /// <summary>Whether <see cref="User"/> is present with identity.</summary>
        public readonly bool HasUserContext =>
            User is UserContext;

        /// <summary>Whether enrichment loaded <see cref="UserContext.Info"/>.</summary>
        public readonly bool HasEnrichedUser =>
            User is UserContext uc && uc.Info is not null;

        public readonly UserContext? EnrichedUserContext =>
            User is UserContext uc && uc.Info is not null ? uc : null;

        public readonly Guid? StreamSessionId =>
            Stream?.Id;

        public readonly bool HasStreamContext =>
            Stream is not null;

        public readonly bool TryGetPersistedUser([NotNullWhen(true)] out User? user)
        {
            user = User is UserContext uc ? uc.Info : null;
            return user is not null;
        }

        public static EventContext From(UserIdentity identity)
        {
            return new EventContext()
            {
                User = new UserContext() { Identity = identity }
            };
        }
    }

    public record struct UserContext
    {
        public required UserIdentity Identity { get; set; }
        public User? Info { get; set; }
        public UserPreferenceSnapshot? PreferenceSnapshot { get; set; }
    }
}
