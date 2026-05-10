using System.Diagnostics.CodeAnalysis;
using Hellbot.Core.Events.Context;
using Hellbot.Core.Entitlements;
using Hellbot.Core.Sessions;
using Hellbot.Core.Users;

namespace Hellbot.Core.Events
{
    public record struct EventContext
    {
        public UserContext? User { get; set; }
        public SenderContext? Sender { get; set; }
        public StreamSessionSnapshot? Stream { get; set; }

        /// <summary>Whether <see cref="User"/> is present (enrichment payload).</summary>
        public readonly bool HasUserContext =>
            User is UserContext uc && uc.Info is not null;

        public readonly bool TryGetUser([NotNullWhen(true)] out User? user)
        {
            user = User is UserContext uc ? uc.Info : null;
            return user is not null;
        }

        public readonly bool HasUserPreferences =>
            User is UserContext uc && uc.PreferenceSnapshot is not null;

        public readonly bool TryGetUserPreferences([NotNullWhen(true)] out UserPreferenceSnapshot? userPreferences)
        {
            userPreferences = User is UserContext uc ? uc.PreferenceSnapshot : null;
            return userPreferences is not null;
        }

        public readonly bool HasStreamContext =>
            Stream is not null;
    }
}
