namespace Hellbot.Service.Stats;

public interface IUserStatsReader
{
    /// <summary>
    /// Persisted value for one scope (<see cref="Hellbot.Service.Data.StatScopeBuckets.ForStreamScope"/>); excludes unsent buffer deltas.
    /// </summary>
    Task<long?> GetForStreamAsync(Guid userId, string statKey, Guid? streamSessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sum of persisted values for this user and key across all stream buckets.
    /// </summary>
    Task<long> GetLifetimeSumAsync(Guid userId, string statKey, CancellationToken cancellationToken = default);
}
