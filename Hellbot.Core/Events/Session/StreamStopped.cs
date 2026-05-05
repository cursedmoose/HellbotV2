namespace Hellbot.Core.Events.Session
{
    public record StreamStopPayload
    {
        public string ChannelId { get; init; } = "api";
    }

    public record StreamStopped : HellbotEvent<StreamStopPayload>;
}
