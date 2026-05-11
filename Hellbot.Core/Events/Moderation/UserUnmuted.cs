namespace Hellbot.Core.Events.Moderation
{
    public record UserUnmutedPayload
    {
        public required string UserId { get; init; }
    }

    public record UserUnmuted : HellbotEvent<UserUnmutedPayload>;
}
