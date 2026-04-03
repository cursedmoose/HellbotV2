namespace Hellbot.Core.Sessions
{
    public interface IStreamSessionManager
    {
        bool IsActive { get; }
        Guid? CurrentSessionId { get; }

        StreamSession StartSession(DateTimeOffset startedAt);
        StreamSession? EndSession(DateTimeOffset endedAt);

        StreamSession? GetCurrentSession();
    }
}
