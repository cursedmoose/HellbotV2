using Hellbot.Core.Users;

namespace Hellbot.Core.Events
{
    public interface IHellbotEvent
    {
        Guid Id { get; init; }
        DateTimeOffset Timestamp { get; init; }
        EventSource Source { get; init; }
        Guid? StreamId { get; set; }

        EventContext Context { get; set; }
    }

    public class EventContext
    {
        public UserContext? UserContext { get; set; }
    }

    public class UserContext
    {
        public required UserIdentity Identity { get; set; }
        public User? Info { get; set; }
    }
    public record UserIdentity
    {
        public required PlatformSource Platform { get; init; }
        public required string UserId { get; init; }
        public string? Username { get; init; }

    }
}
