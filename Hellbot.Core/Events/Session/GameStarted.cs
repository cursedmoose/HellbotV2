namespace Hellbot.Core.Events.Session
{
    public record GameStartedPayload;
    public record GameStarted : HellbotEvent<GameStartedPayload>;
}
