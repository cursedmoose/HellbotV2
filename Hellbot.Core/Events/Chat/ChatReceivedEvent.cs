using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Hellbot.Core.Events.Chat
{
    public class ChatReceivedEvent(string id, EventSource eventSource, string message, string user) : IHellbotEvent
    {
        public string Id { get; init; } = id;
        public DateTime Timestamp { get; init; } = DateTime.Now;
        public EventSource Source { get; init; } = eventSource;

        public string Message { get; init; } = message;
        public string User { get; init; } = user;
        public IReadOnlyDictionary<string, object>? Metadata { get; init; } = new Dictionary<string, object>()
        {
            { "Message", message },
            { "User", user }
        };
    }
}
