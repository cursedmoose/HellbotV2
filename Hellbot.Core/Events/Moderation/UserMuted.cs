namespace Hellbot.Core.Events.Moderation
{
    public record UserMutedPayload
    {
        public required string UserId { get; init; }
        public required DateTimeOffset ExpiresAt { get; init; }
        public string? Reason { get; init; }
    }

    public record UserMuted : HellbotEvent<UserMutedPayload>;
}
