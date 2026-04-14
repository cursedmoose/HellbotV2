using Hellbot.Core.Users;

namespace Hellbot.Core.Events
{
    public record struct EventContext
    {
        public UserContext? UserContext { get; set; }

        public static EventContext From(UserIdentity identity)
        {
            return new EventContext()
            {
                UserContext = new UserContext() { Identity = identity }
            };
        }
    }

    public record struct UserContext
    {
        public required UserIdentity Identity { get; set; }
        public User? Info { get; set; }
    }
}
