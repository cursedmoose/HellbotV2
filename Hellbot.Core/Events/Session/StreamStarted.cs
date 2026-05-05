namespace Hellbot.Core.Events.Session
{
    public record StreamStartPayload
    {
        public string ChannelId { get; init; } = "api";
        public string? Title { get; init; }
        public string? GameName { get; init; }
        public string? Description { get; init; }
        public string? ExternalBroadcastId { get; init; }
        public string? DestinationUrl { get; init; }
    }

    public record StreamStarted : HellbotEvent<StreamStartPayload>;
}
