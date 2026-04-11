namespace Hellbot.Core.Events.Users
{
    public record UserUnbannedPayload
    {
        public required string UserId;

    };
    public record UserUnbanned : HellbotEvent<UserUnbannedPayload>;
}
