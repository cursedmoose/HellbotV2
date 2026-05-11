namespace Hellbot.Core.Events
{
    public enum PlatformSource
    {
        None = 0,
        Test,
        API,
        Twitch,
        Hellbot,
        Playnite,
        OBS,
        StreamSession
    }

    public sealed record EventSource(PlatformSource Platform, string? Channel = null)
    {
        public static readonly EventSource Test = new(PlatformSource.Test);
        public static readonly EventSource API = new(PlatformSource.API);
        public static readonly EventSource Twitch = new(PlatformSource.Twitch);
        public static readonly EventSource Internal = new(PlatformSource.Hellbot);
        public static readonly EventSource Playnite = new(PlatformSource.Playnite);
        public static readonly EventSource OBS = new(PlatformSource.OBS);


        public override string ToString()
            => Channel is null
            ? PlatformSource.GetName(Platform)!
            : $"{PlatformSource.GetName(Platform)!}:{Channel}";
    }
}
