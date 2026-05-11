namespace Hellbot.Core.Users
{
    public record User
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Status { get; init; } = Standing.Active;
        public Role Role { get; init; } = Role.None;
        public DateTimeOffset? JoinedAt { get; init; }

        public bool Joined => JoinedAt.HasValue;
        public bool InGoodStanding => string.Equals(Status, Standing.Active, StringComparison.Ordinal);
    }
}
