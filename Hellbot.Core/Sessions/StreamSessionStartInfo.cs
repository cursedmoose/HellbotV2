using Hellbot.Core.Events;

namespace Hellbot.Core.Sessions
{
    public record StreamSessionStartInfo(
        DateTimeOffset StartedAt,
        PlatformSource SourcePlatform,
        StreamMetadata Metadata,
        StreamDestination Destination);
}
