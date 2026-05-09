namespace Hellbot.Service.Data;

/// <summary>
/// Encodes persisted <c>scope</c> column values for <c>user_stats</c>.
/// </summary>
public static class StatScopeBuckets
{
    public const string StreamPrefix = "stream:";

    public static string ForStreamSession(Guid streamSessionId) => $"{StreamPrefix}{streamSessionId:D}";
}
