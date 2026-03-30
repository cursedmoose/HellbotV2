namespace Hellbot.Core.Events
{
    public interface IHellbotEvent
    {
        DateTime Timestamp { get; init; }
        string EventSource { get; init; }
    }
}
