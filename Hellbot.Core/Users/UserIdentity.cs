using Hellbot.Core.Events;

namespace Hellbot.Core.Users
{
    public readonly record struct UserIdentity
    {
        public required PlatformSource Platform { get; init; }
        public required string UserId { get; init; }
        public string? Username { get; init; }
    }
}
