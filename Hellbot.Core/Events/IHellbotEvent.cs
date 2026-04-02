namespace Hellbot.Core.Events
{
    public interface IHellbotEvent
    {
        string Id { get; init; }
        DateTime Timestamp { get; init; }
        EventSource Source { get; init; }
        IReadOnlyDictionary<string, object>? Metadata { get; init; }
    }
}
