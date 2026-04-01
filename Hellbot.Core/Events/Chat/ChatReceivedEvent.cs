namespace Hellbot.Core.Events.Chat
{
    public class ChatReceivedEvent(string eventSource, string message, string user) : IHellbotEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.Now;
        public string EventSource { get; init; } = eventSource;
        public string Message { get; init; } = message;
        public string User { get; init; } = user;
    }
}
