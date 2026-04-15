using Hellbot.Core.Commands;

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
}
