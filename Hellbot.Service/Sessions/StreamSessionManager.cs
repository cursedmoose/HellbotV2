using Hellbot.Core.Events;
using Hellbot.Core.Sessions;
using Hellbot.Service.Clients.Twitch;
using Hellbot.Service.Config;
using Microsoft.Extensions.Options;

namespace Hellbot.Service.Sessions
{
    public class StreamSessionManager(IOptions<StreamSessionOptions> options, TwitchClient twitch) : IStreamSessionManager
    {
        private readonly Lock _lock = new();
        private StreamSession? _currentSession;
        private StreamSessionSnapshot? _streamSnapshot;

        public bool IsActive => _currentSession?.IsActive == true;
        public Guid? CurrentSessionId => _currentSession?.Id;

        public StreamSessionSnapshot? CurrentStreamSnapshot => _streamSnapshot;

        public StreamSession? GetCurrentSession() => _currentSession;

        public async Task UpdateChannelAsync(string? gameId, string? title)
        {
            await twitch.ModifyChannelInformationAsync(gameId, title);

            lock (_lock)
            {
                if (_currentSession is not { IsActive: true })
                    return;

                if (!string.IsNullOrEmpty(title))
                    _currentSession.Metadata = _currentSession.Metadata with { Title = title };

                RefreshSnapshot();
            }
        }

        public StreamSession StartOrAddDestination(StreamSessionStartInfo info)
        {
            lock (_lock)
            {
                if (_currentSession != null && _currentSession.IsActive)
                {
                    ApplyMetadata(_currentSession, info.Metadata, info.SourcePlatform);
                    TryAddDestination(_currentSession, info.Destination);
                    RefreshSnapshot();
                    return _currentSession;
                }

                _currentSession = new StreamSession
                {
                    Id = Guid.NewGuid(),
                    StartedAt = info.StartedAt,
                    Metadata = new StreamMetadata()
                };
                ApplyMetadata(_currentSession, info.Metadata, info.SourcePlatform);
                TryAddDestination(_currentSession, info.Destination);
                RefreshSnapshot();
                return _currentSession;
            }
        }

        public bool RemoveDestination(StreamSessionStopInfo info, DateTimeOffset stoppedAt, out StreamSession? endedSession)
        {
            endedSession = null;
            lock (_lock)
            {
                if (_currentSession == null || !_currentSession.IsActive)
                    return false;

                var list = _currentSession.Destinations;
                var removed = list.RemoveAll(d =>
                    d.Platform == info.Platform &&
                    string.Equals(d.ChannelId, info.ChannelId, StringComparison.Ordinal));

                if (removed == 0)
                    return false;

                if (list.Count == 0)
                {
                    _currentSession.EndedAt = stoppedAt;
                    endedSession = _currentSession;
                    _currentSession = null;
                }

                RefreshSnapshot();
                return true;
            }
        }

        private void RefreshSnapshot()
            => _streamSnapshot = _currentSession != null && _currentSession.IsActive
                ? StreamSessionSnapshot.From(_currentSession)
                : null;

        private void ApplyMetadata(StreamSession session, StreamMetadata incoming, PlatformSource source)
        {
            if (source == options.Value.MetadataSourceOfTruth)
            {
                if (!string.IsNullOrEmpty(incoming.Title))
                    session.Metadata = session.Metadata with { Title = incoming.Title };
                if (!string.IsNullOrEmpty(incoming.GameName))
                    session.Metadata = session.Metadata with { GameName = incoming.GameName };
                if (!string.IsNullOrEmpty(incoming.Description))
                    session.Metadata = session.Metadata with { Description = incoming.Description };
                return;
            }

            if (!string.IsNullOrEmpty(incoming.Title) && string.IsNullOrEmpty(session.Metadata.Title))
                session.Metadata = session.Metadata with { Title = incoming.Title };
            if (!string.IsNullOrEmpty(incoming.GameName) && string.IsNullOrEmpty(session.Metadata.GameName))
                session.Metadata = session.Metadata with { GameName = incoming.GameName };
            if (!string.IsNullOrEmpty(incoming.Description) && string.IsNullOrEmpty(session.Metadata.Description))
                session.Metadata = session.Metadata with { Description = incoming.Description };
        }

        private static void TryAddDestination(StreamSession session, StreamDestination destination)
        {
            if (session.Destinations.Any(d =>
                    d.Platform == destination.Platform &&
                    string.Equals(d.ChannelId, destination.ChannelId, StringComparison.Ordinal)))
                return;

            session.Destinations.Add(destination);
        }
    }
}
