namespace Hellbot.Core.Events.Moderation
{
    public record UserBannedPayload
    {
        public required string UserId { get; init; }
        public required string Reason { get; init; }
        public required DateTimeOffset BannedAt { get; init; }
    }

    public record UserBanned : HellbotEvent<UserBannedPayload>;
}
