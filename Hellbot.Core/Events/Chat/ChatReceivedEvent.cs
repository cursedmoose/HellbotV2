namespace Hellbot.Core.Events.Chat
{
    public class ChatReceivedEvent(EventSource eventSource, string message, string user) : IHellbotEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.Now;
        public EventSource Source { get; init; } = eventSource;
        public string Message { get; init; } = message;
        public string User { get; init; } = user;
    }
}
