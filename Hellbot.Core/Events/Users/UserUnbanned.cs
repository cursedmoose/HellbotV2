namespace Hellbot.Core.Events.Users
{
    public record UserUnbannedPayload
    {
        public required string UserId { get; init; }
    };
    public record UserUnbanned : HellbotEvent<UserUnbannedPayload>;
}
