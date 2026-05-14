using Hellbot.Core.Events;
using Hellbot.Core.Sessions;
using Hellbot.Service.Config;
using Hellbot.Service.Sessions;
using Microsoft.Extensions.Options;

namespace Hellbot.Service.Clients.Twitch
{
    public sealed class TwitchStreamingChannelUpdater(IOptions<TwitchOptions> options, TwitchClient twitch)
        : IStreamingChannelUpdater
    {
        public PlatformSource Platform => PlatformSource.Twitch;

        public Task UpdateAsync(StreamDestination destination, string? gameId, string? title,
            CancellationToken cancellationToken = default)
        {
            if (!string.Equals(destination.ChannelId, options.Value.BroadcasterId, StringComparison.Ordinal))
                return Task.CompletedTask;

            return twitch.ModifyChannelInformationAsync(options.Value.BroadcasterId, gameId, title);
        }
    }
}
