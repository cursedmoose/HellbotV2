namespace Hellbot.Core.Events.Users
{
    public record UserUnmoddedPayload
    {
        public required string UserId { get; init; }
    }

    public record UserUnmodded : HellbotEvent<UserUnmoddedPayload>;
}
