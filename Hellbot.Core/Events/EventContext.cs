using Hellbot.Core.Entitlements;
using Hellbot.Core.Sessions;
using Hellbot.Core.Users;

namespace Hellbot.Core.Events
{
    public record struct EventContext
    {
        public UserContext? User { get; set; }
        public StreamSessionSnapshot? Stream { get; set; }

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
