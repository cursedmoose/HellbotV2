namespace Hellbot.Core.Events.Users
{

    public record UserBannedPayload
    {
        public required string UserId { get; init; }
        public required string Reason { get; init; }
        public required DateTimeOffset BannedAt { get; init; }
        public required bool IsPermanent { get; init; }
    };
    public record UserBanned : HellbotEvent<UserBannedPayload>;
}
