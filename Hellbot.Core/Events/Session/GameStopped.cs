namespace Hellbot.Core.Events.Session
{
    public record GameStoppedPayload
    {
        public required string Name { get; init; }
        public required string Id { get; init; }
    };
    public record GameStopped : HellbotEvent<GameStoppedPayload>;
}
