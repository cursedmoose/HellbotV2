using System.Collections.Generic;

namespace Hellbot.Service.Config
{
    public class ElevenLabsOptions
    {
        public string ApiKey { get; set; } = "";

        // JSON keys cannot use ':' in the path segment; runtime ids may use ':' if lookup normalizes like ObsClient.
        public Dictionary<string, VoiceCatalogEntry> Voices { get; set; } = new();
    }
}
