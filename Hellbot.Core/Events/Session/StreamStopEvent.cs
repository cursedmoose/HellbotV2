namespace Hellbot.Core.Events.Session
{
    public record StreamStopPayload;
    public record StreamStopEvent : HellbotEvent<StreamStopPayload>;
}
