namespace Hellbot.Core.Users
{
    public record User
    {
        public Guid Id { get; init; } = new Guid();
        public string? TwitchId { get; init; }
    }
}
