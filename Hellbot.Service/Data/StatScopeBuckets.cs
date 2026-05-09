namespace Hellbot.Service.Data;

/// <summary>
/// Encodes persisted <c>scope</c> column values for <c>user_stats</c>.
/// </summary>
public static class StatScopeBuckets
{
    public const string StreamPrefix = "stream:";

    /// <summary>No attributed stream session — e.g. chat while offline (“Why are you still here?”).</summary>
    public const string OfflineScope = $"{StreamPrefix}null";

    /// <summary>
    /// <paramref name="streamSessionId"/> null → <see cref="OfflineScope"/>.
    /// Otherwise → <c>stream:{'{guid}'}</c> (includes <see cref="Guid.Empty"/> if that ever appears).
    /// </summary>
    public static string ForStreamScope(Guid? streamSessionId) =>
        streamSessionId is { } id
            ? $"{StreamPrefix}{id:D}"
            : OfflineScope;
}
