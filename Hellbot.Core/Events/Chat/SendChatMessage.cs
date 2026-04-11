namespace Hellbot.Core.Events.Chat
{
    public record SendChatMessage : HellbotEvent<SendChatPayload>;

    public record SendChatPayload
    {
        public required string Message { get; init; }
        public string? ReplyTo { get; init; } = default;
        public required EventSource Channel { get; init; }

        public bool IsReply => string.IsNullOrEmpty(ReplyTo);
    };
}
