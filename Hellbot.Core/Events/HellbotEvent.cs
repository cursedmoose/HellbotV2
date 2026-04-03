namespace Hellbot.Core.Events
{
    public abstract record HellbotEvent<TPayload> : IHellbotEvent
    {
        public Guid Id { get; init; } = Guid.CreateVersion7();
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
        public required EventSource Source { get; init; }
        public required TPayload Data { get; init; }
    }
}
