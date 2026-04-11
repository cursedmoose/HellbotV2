namespace Hellbot.Core.Events.Session
{
    public record GameStoppedPayload;
    public record GameStopped : HellbotEvent<GameStoppedPayload>;
}
