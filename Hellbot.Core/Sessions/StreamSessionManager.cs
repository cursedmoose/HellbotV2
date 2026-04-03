namespace Hellbot.Core.Sessions
{
    public class StreamSessionManager : IStreamSessionManager
    {
        private readonly Lock _lock = new();
        private StreamSession? _currentSession;
        public bool IsActive => _currentSession?.IsActive == true;
        public Guid? CurrentSessionId => _currentSession?.Id;
        public StreamSession? GetCurrentSession() => _currentSession;

        public StreamSession StartSession(DateTimeOffset startedAt)
        {
            lock (_lock)
            {
                if (_currentSession?.IsActive == true)
                {
                    return _currentSession;
                }

                _currentSession = new StreamSession
                {
                    Id = Guid.NewGuid(),
                    StartedAt = startedAt
                };

                return _currentSession;
            }
        }

        public StreamSession? EndSession(DateTimeOffset endedAt)
        {
            lock (_lock)
            {
                if (_currentSession == null || !_currentSession.IsActive)
                {
                    return null;
                }

                _currentSession.EndedAt = endedAt;
                var endedSession = _currentSession;
                _currentSession = null;
                return endedSession;
            }
        }
    }
}
