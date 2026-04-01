namespace Hellbot.Service.Config
{
    public class TwitchOptions
    {
        public TwitchApiOptions API { get; set; } = new TwitchApiOptions();
        public string Channel { get; set; } = "";
        public string ChannelId { get; set; } = "";
        public string BroadcasterId { get; set; } = "";
    }

    public class TwitchApiOptions
    {
        public string ClientId { get; set; } = "";
        public string RedirectUrl { get; set; } = "";
        public string ClientSecret { get; set; } = "";
    }
}
