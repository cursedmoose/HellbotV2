namespace Hellbot.Core.Events
{
    public interface IHellbotEvent
    {
        DateTime Timestamp { get; init; }
        EventSource Source { get; init; }
    }
}
