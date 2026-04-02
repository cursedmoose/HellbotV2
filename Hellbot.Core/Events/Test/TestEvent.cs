namespace Hellbot.Core.Events.Test
{
    public abstract class TestEvent : IHellbotEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.Now;
        public EventSource Source { get; init; } = EventSource.Test;
    }
}
