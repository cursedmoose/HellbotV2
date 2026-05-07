namespace Hellbot.Core.Events.Users
{
    public record UserFollowedPayload
    {
        public required string FollowerUserId { get; init; }
        public required string FollowerUserName { get; init; }
        public required DateTimeOffset FollowedAt { get; init; }
    }

    public record UserFollowed : HellbotEvent<UserFollowedPayload>;
}
