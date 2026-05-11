namespace Hellbot.Core.Events.Users
{
    public record UserModdedPayload
    {
        public required string UserId { get; init; }
    }

    public record UserModded : HellbotEvent<UserModdedPayload>;
}
