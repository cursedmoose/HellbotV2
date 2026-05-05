using Hellbot.Core.Sessions;

namespace Hellbot.Service.Sessions
{
    public interface IStreamSessionManager
    {
        bool IsActive { get; }
        Guid? CurrentSessionId { get; }

        StreamSessionSnapshot? CurrentStreamSnapshot { get; }

        StreamSession StartOrAddDestination(StreamSessionStartInfo info);
        bool RemoveDestination(StreamSessionStopInfo info, DateTimeOffset stoppedAt, out StreamSession? endedSession);

        StreamSession? GetCurrentSession();
    }
}
