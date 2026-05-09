using Hellbot.Service.Data.Tables;

namespace Hellbot.Service.Stats;

public sealed class UserStatsReader(UserStatTable table) : IUserStatsReader
{
    public Task<long?> GetForStreamAsync(
        Guid userId,
        string statKey,
        Guid streamSessionId,
        CancellationToken cancellationToken = default)
        => table.GetForStreamAsync(userId, statKey, streamSessionId, cancellationToken);

    public Task<long> GetLifetimeSumAsync(
        Guid userId,
        string statKey,
        CancellationToken cancellationToken = default)
        => table.GetLifetimeSumAsync(userId, statKey, cancellationToken);
}
