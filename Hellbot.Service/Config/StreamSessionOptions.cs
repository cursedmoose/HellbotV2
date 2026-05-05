using Hellbot.Core.Events;

namespace Hellbot.Service.Config
{
    public class StreamSessionOptions
    {
        public PlatformSource MetadataSourceOfTruth { get; set; } = PlatformSource.Twitch;
    }
}
