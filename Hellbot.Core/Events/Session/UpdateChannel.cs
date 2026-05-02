namespace Hellbot.Core.Events.Session
{
    public record UpdateChannelPayload
    {
        public string? GameId { get; init; }
        public string? Title { get; init; }
    };
    public record UpdateChannel : HellbotEvent<UpdateChannelPayload>;
}
