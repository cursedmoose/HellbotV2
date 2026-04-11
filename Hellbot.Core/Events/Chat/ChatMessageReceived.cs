namespace Hellbot.Core.Events.Chat
{
    public record ChatReceivedPayload
    {
        public required string User { get; init; }
        public required string Message { get; init; }
        public required string Channel { get; init; }
    }

    public record ChatMessageReceived : HellbotEvent<ChatReceivedPayload>;
}
