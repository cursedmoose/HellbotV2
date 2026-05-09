namespace Hellbot.Service.Stats;

public interface IUserStatsRecorder
{
    /// <summary>
    /// Adds <paramref name="delta"/> to the counter for this user, key, and stream session.
    /// No-op if <paramref name="streamSessionId"/> is null — counters are stream-scoped only.
    /// </summary>
    void Increment(Guid userId, string statKey, long delta = 1, Guid? streamSessionId = null);
}
