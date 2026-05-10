using Hellbot.Core.Tts;

namespace Hellbot.Service.Config
{
    /// <summary>Maps an equipped entitlement id to an ElevenLabs voice id and optional settings.</summary>
    public class VoiceCatalogEntry
    {
        public string Id { get; set; } = "";

        /// <summary>Omit in config to use <see cref="Hellbot.Core.Tts.VoiceSettings"/> constructor defaults at synthesis time.</summary>
        public VoiceSettings? Settings { get; set; }
    }
}
