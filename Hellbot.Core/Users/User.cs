using Hellbot.Core.Commands;
using Hellbot.Core.Events;

namespace Hellbot.Core.Users
{
    public record User
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Status { get; init; } = "Active";
        public Role Role { get; init; } = Role.None;
        public DateTimeOffset? JoinedAt { get; init; }

        public bool Joined => JoinedAt.HasValue;
    }

    public readonly record struct UserIdentity
    {
        public required PlatformSource Platform { get; init; }
        public required string UserId { get; init; }
        public string? Username { get; init; }
    }
}
