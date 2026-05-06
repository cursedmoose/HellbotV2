using System.Text.Json;
using Hellbot.Core.Users;

namespace Hellbot.Core.Events
{
    /// <summary>
    /// Payload for <c>ReceiveEvent</c> on the SignalR events hub;
    /// sent by Hellbot.Service <c>SignalREventBroadcaster</c>.
    /// </summary>
    public sealed record HubEventMessage
    {
        public Guid Id { get; init; }
        public string Type { get; init; } = "";
        public DateTimeOffset Timestamp { get; init; }
        public required EventSource Source { get; init; }
        public UserIdentity? User { get; init; }
        public JsonElement Data { get; init; }
    }
}
