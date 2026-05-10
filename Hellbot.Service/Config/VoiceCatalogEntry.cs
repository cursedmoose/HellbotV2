namespace Hellbot.Service.Config
{
    /// <summary>Maps an equipped entitlement id to an ElevenLabs voice id and optional settings overrides.</summary>
    public class VoiceCatalogEntry
    {
        public string Id { get; set; } = "";

        /// <summary>Omitted properties use defaults from <see cref="Hellbot.Core.Tts.VoiceSettings"/>.</summary>
        public VoiceCatalogEntrySettings? Settings { get; set; }
    }

    /// <summary>Optional overrides aligned with <see cref="Hellbot.Core.Tts.VoiceSettings"/>; omit keys for defaults.</summary>
    public class VoiceCatalogEntrySettings
    {
        public float? Stability { get; set; }
        public float? SimilarityBoost { get; set; }
        public float? Style { get; set; }
        public bool? UseSpeakerBoost { get; set; }
        public float? Speed { get; set; }
    }
}
