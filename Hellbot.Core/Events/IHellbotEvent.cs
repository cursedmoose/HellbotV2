namespace Hellbot.Core.Events
{
    public interface IHellbotEvent
    {
        Guid Id { get; init; }
        DateTimeOffset Timestamp { get; init; }
        EventSource Source { get; init; }
        Guid? StreamId { get; set; }
    }
}
