using Dapper;
using Hellbot.Service.Data;
using System.Data;

namespace Hellbot.Service.Data.Tables;

public class UserStatTable(IDbConnectionFactory factory)
{
    private const string UpsertDeltaSql =
        """
        INSERT INTO user_stat_counters (user_id, stat_key, scope_bucket, value, updated_at)
        VALUES (@UserId, @StatKey, @ScopeBucket, @Delta, @UpdatedAt)
        ON CONFLICT (user_id, stat_key, scope_bucket)
        DO UPDATE SET
            value = user_stat_counters.value + excluded.value,
            updated_at = excluded.updated_at
        """;

    public async Task ApplyDeltaBatchAsync(
        IReadOnlyDictionary<(Guid UserId, string StatKey, string ScopeBucket), long> deltas,
        CancellationToken cancellationToken = default)
    {
        if (deltas.Count == 0)
            return;

        var now = DateTime.UtcNow;
        using var connection = factory.CreateConnection();
        using var transaction = connection.BeginTransaction();
        foreach (var ((userId, statKey, scopeBucket), delta) in deltas)
        {
            if (delta == 0)
                continue;

            await connection.ExecuteAsync(
                new CommandDefinition(
                    UpsertDeltaSql,
                    new
                    {
                        UserId = userId,
                        StatKey = statKey,
                        ScopeBucket = scopeBucket,
                        Delta = delta,
                        UpdatedAt = now,
                    },
                    transaction,
                    cancellationToken: cancellationToken));
        }

        transaction.Commit();
    }

    public async Task<long?> GetForStreamAsync(
        Guid userId,
        string statKey,
        Guid streamSessionId,
        CancellationToken cancellationToken = default)
    {
        using var connection = factory.CreateConnection();
        var scopeBucket = StatScopeBuckets.ForStreamSession(streamSessionId);

        return await connection.ExecuteScalarAsync<long?>(
            new CommandDefinition(
                """
                SELECT value
                FROM user_stat_counters
                WHERE user_id = @UserId AND stat_key = @StatKey AND scope_bucket = @ScopeBucket
                """,
                new { UserId = userId, StatKey = statKey, ScopeBucket = scopeBucket },
                cancellationToken: cancellationToken));
    }

    public async Task<long> GetLifetimeSumAsync(Guid userId, string statKey, CancellationToken cancellationToken = default)
    {
        using var connection = factory.CreateConnection();

        var sum = await connection.ExecuteScalarAsync<long?>(
            new CommandDefinition(
                """
                SELECT COALESCE(SUM(value), 0)
                FROM user_stat_counters
                WHERE user_id = @UserId AND stat_key = @StatKey
                """,
                new { UserId = userId, StatKey = statKey },
                cancellationToken: cancellationToken));

        return sum ?? 0;
    }
}
