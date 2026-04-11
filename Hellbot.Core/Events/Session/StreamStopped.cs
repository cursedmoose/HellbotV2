namespace Hellbot.Core.Events.Session
{
    public record StreamStopPayload;
    public record StreamStopped : HellbotEvent<StreamStopPayload>;
}
