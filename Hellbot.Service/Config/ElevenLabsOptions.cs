using System.Collections.Generic;

namespace Hellbot.Service.Config
{
    public class ElevenLabsOptions
    {
        public string ApiKey { get; set; } = "";

        public Dictionary<string, VoiceCatalogEntry> Voices { get; set; } = new();
    }
}
