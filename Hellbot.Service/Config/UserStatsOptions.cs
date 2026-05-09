namespace Hellbot.Service.Config;

public sealed class UserStatsOptions
{
    public TimeSpan FlushInterval { get; set; } = TimeSpan.FromMinutes(1);
}
