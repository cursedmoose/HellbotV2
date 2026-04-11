namespace Hellbot.Core.Events.Session
{
    public record StreamStartPayload;
    public record StreamStarted : HellbotEvent<StreamStartPayload>;
}
