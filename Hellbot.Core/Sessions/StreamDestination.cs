using Hellbot.Core.Events;

namespace Hellbot.Core.Sessions
{
    public record StreamDestination(
        PlatformSource Platform,
        string ChannelId,
        DateTimeOffset StartedAt,
        string? ExternalBroadcastId = null,
        string? Url = null);
}
