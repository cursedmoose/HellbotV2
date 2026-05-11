namespace Hellbot.Core.Events.Moderation
{
    public record UserUnbannedPayload
    {
        public required string UserId { get; init; }
    }

    public record UserUnbanned : HellbotEvent<UserUnbannedPayload>;
}
