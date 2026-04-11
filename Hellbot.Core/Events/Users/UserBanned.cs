namespace Hellbot.Core.Events.Users
{

    public record UserBannedPayload
    {
        public required string UserId;
        public required string Reason;
        public required DateTimeOffset BannedAt;
        public required bool IsPermanent;
    };
    public record UserBanned : HellbotEvent<UserBannedPayload>;
}
