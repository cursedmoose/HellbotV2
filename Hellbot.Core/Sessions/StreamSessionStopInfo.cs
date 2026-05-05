using Hellbot.Core.Events;

namespace Hellbot.Core.Sessions
{
    public record StreamSessionStopInfo(
        PlatformSource Platform,
        string ChannelId);
}
