namespace Hellbot.Core.Events
{
    public enum PlatformSource
    {
        None = 0,
        Test,
        API,
        Twitch
    }

    public sealed record EventSource(PlatformSource Platform, string? Channel = null)
    {
        public static readonly EventSource Test = new(PlatformSource.Test);
        public static readonly EventSource API = new(PlatformSource.API);
        public static readonly EventSource Twitch = new(PlatformSource.Twitch);

        public override string ToString()
            => Channel is null
            ? PlatformSource.GetName(Platform)!
            : $"{PlatformSource.GetName(Platform)!}:{Channel}";
    }
}
