using Hellbot.Service.Data;
using Hellbot.Service.Data.Tables;

namespace Hellbot.Service.Stats;

public sealed class UserStatsRecorder(ILogger<UserStatsRecorder> logger) : IUserStatsRecorder
{
    private readonly object _bufferLock = new();
    private Dictionary<(Guid UserId, string StatKey, string ScopeBucket), long> _pending = new();

    public void Increment(Guid userId, string statKey, long delta = 1, Guid? streamSessionId = null)
    {
        if (delta == 0)
            return;

        ArgumentException.ThrowIfNullOrWhiteSpace(statKey);

        var bucket = StatScopeBuckets.ForStreamScope(streamSessionId);
        var key = (userId, statKey, bucket);

        lock (_bufferLock)
        {
            if (_pending.TryGetValue(key, out var current))
                _pending[key] = current + delta;
            else
                _pending[key] = delta;
        }
    }

    public Task FlushPendingAsync(UserStatTable table, CancellationToken cancellationToken)
    {
        Dictionary<(Guid UserId, string StatKey, string ScopeBucket), long> snapshot;
        lock (_bufferLock)
        {
            if (_pending.Count == 0)
                return Task.CompletedTask;

            snapshot = _pending;
            _pending = new Dictionary<(Guid UserId, string StatKey, string ScopeBucket), long>();
        }

        logger.LogTrace("Flushing user stat deltas: RowCount={Count}", snapshot.Count);
        return table.ApplyDeltaBatchAsync(snapshot, cancellationToken);
    }
}
