namespace Hellbot.Core.Events.Chat
{
    public record ChatMessageDeleted : HellbotEvent<ChatDeletedPayload>;

    public record ChatDeletedPayload
    {
        public required string MessageId;
    };
}
