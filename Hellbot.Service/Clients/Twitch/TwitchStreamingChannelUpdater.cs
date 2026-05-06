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
        private string EffectiveBroadcasterId =>
            string.IsNullOrEmpty(options.Value.BroadcasterId)
                ? options.Value.ChannelId
                : options.Value.BroadcasterId;

        public PlatformSource Platform => PlatformSource.Twitch;

        public Task UpdateAsync(StreamDestination destination, string? gameId, string? title,
            CancellationToken cancellationToken = default)
        {
            if (!string.Equals(destination.ChannelId, EffectiveBroadcasterId, StringComparison.Ordinal))
                return Task.CompletedTask;

            return twitch.ModifyChannelInformationAsync(destination.ChannelId, gameId, title);
        }
    }
}
