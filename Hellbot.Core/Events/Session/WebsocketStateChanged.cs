namespace Hellbot.Core.Events.Session
{
    public enum ConnectionState
    {
        Initialized,
        Connected,
        Disconnected
    }
    public record WebsocketStatePayload
    {
        public ConnectionState Status { get; init; } = ConnectionState.Initialized;
        public string? Details { get; init; }
    };
    public record WebsocketStateChanged : HellbotEvent<WebsocketStatePayload>;
}
