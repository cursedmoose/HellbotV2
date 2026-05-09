namespace Hellbot.Service.Data;

/// <summary>
/// Encodes persisted <c>scope_bucket</c> values for <c>user_stat_counters</c>.
/// </summary>
public static class StatScopeBuckets
{
    public const string StreamPrefix = "stream:";

    public static string ForStreamSession(Guid streamSessionId) => $"{StreamPrefix}{streamSessionId:D}";
}
