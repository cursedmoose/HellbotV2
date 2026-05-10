namespace Hellbot.Core.Events.Users
{
    public record UserFollowedPayload
    {
        public required DateTimeOffset FollowedAt { get; init; }
    }

    public record UserFollowed : HellbotEvent<UserFollowedPayload>;
}
