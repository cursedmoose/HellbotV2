namespace Hellbot.Service.Stats;

public interface IUserStatsRecorder
{
    /// <summary>
    /// Adds <paramref name="delta"/> for this user and stat key.
    /// <paramref name="streamSessionId"/> null → offline bucket <c>stream:null</c> (<see cref="Hellbot.Service.Data.StatScopeBuckets.OfflineScope"/>); otherwise <c>stream:{'{'guid'}'}</c> per <see cref="Hellbot.Service.Data.StatScopeBuckets.ForStreamScope"/>.
    /// </summary>
    void Increment(Guid userId, string statKey, long delta = 1, Guid? streamSessionId = null);
}
