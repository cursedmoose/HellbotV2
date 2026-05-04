namespace Hellbot.UI.Configuration
{
    public sealed class HellbotApiOptions
    {
        public const string SectionName = "HellbotApi";

        /// <summary>
        /// Base URL for Hellbot.Service (HTTP API and SignalR hub).
        /// </summary>
        public string BaseUrl { get; set; } = "http://localhost:5131";
    }
}
