using Hellbot.Core.Events;
using Hellbot.Core.Sessions;

namespace Hellbot.Service.Sessions
{
    public interface IStreamingChannelUpdater
    {
        PlatformSource Platform { get; }

        Task UpdateAsync(StreamDestination destination, string? gameId, string? title,
            CancellationToken cancellationToken = default);
    }
}
