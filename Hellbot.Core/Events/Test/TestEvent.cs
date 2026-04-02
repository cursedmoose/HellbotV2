namespace Hellbot.Core.Events.Test
{
    public abstract class TestEvent : IHellbotEvent
    {
        public string Id { get; init; } = Guid.CreateVersion7().ToString();
        public DateTime Timestamp { get; init; } = DateTime.Now;
        public EventSource Source { get; init; } = EventSource.Test;
        public abstract IReadOnlyDictionary<string, object>? Metadata { get; init; }
    }
}
