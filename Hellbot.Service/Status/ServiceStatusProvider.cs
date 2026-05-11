using System.Collections.Concurrent;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Session;

namespace Hellbot.Service.Status
{
    /// <summary>
    /// Central snapshot for operator-facing health/status; websocket producer rows are seeded here first.
    /// </summary>
    public sealed class ServiceStatusProvider
    {
        private readonly ConcurrentDictionary<PlatformSource, WebsocketProducerStatusRow> _websocket =
            new();

        public ServiceStatusProvider()
        {
            foreach (var platform in TrackedWebsocketPlatforms)
            {
                _websocket[platform] = new WebsocketProducerStatusRow(
                    platform,
                    ConnectionState.Initialized,
                    Details: null,
                    LastChanged: DateTimeOffset.UtcNow);
            }
        }

        internal static readonly PlatformSource[] TrackedWebsocketPlatforms =
        [
            PlatformSource.Twitch,
            PlatformSource.Playnite,
            PlatformSource.OBS,
            PlatformSource.StreamSession
        ];

        /// <summary>
        /// Default until connect/disconnect events arrive for each producer.
        /// </summary>
        public void RecordWebsocketStatus(EventSource source, WebsocketStatePayload data, DateTimeOffset timestamp)
        {
            if (!_websocket.ContainsKey(source.Platform))
                return;

            _websocket[source.Platform] = new WebsocketProducerStatusRow(
                source.Platform,
                data.Status,
                data.Details,
                timestamp);
        }

        public IReadOnlyList<WebsocketProducerStatusRow> GetWebsocketProducerStatuses()
        {
            return TrackedWebsocketPlatforms
                .Select(p => _websocket[p])
                .ToList();
        }
    }

    public sealed record WebsocketProducerStatusRow(
        PlatformSource Platform,
        ConnectionState Status,
        string? Details,
        DateTimeOffset LastChanged);
}
