namespace Hellbot.Core.Events.Session
{
    public record GameStartedPayload
    {
        public required string Name { get; init; }
        public required string Id { get; init; }
    };
    public record GameStarted : HellbotEvent<GameStartedPayload>;
}
