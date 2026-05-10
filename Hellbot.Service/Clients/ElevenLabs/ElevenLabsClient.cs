using External = ElevenLabs;
using Hellbot.Core.Tts;
using Hellbot.Service.Config;
using Microsoft.Extensions.Options;

namespace Hellbot.Service.Clients.ElevenLabs
{
    public class ElevenLabsClient(IOptions<ElevenLabsOptions> options)
    {
        public readonly External.ElevenLabsClient API = new(options.Value.ApiKey);

        public Task<byte[]> GenerateTts(
            string voiceKey,
            string text,
            VoiceSettings? userVoiceSettings,
            CancellationToken cancellationToken = default)
        {
            var voices = options.Value.Voices;
            if (!voices.TryGetValue(voiceKey, out var entry) || entry is null || string.IsNullOrWhiteSpace(entry.Id))
                throw new InvalidOperationException($"Unknown TTS voice key: {voiceKey}");

            VoiceSettings settings;
            if (userVoiceSettings is not null)
            {
                settings = userVoiceSettings;
            }
            else
            {
                var defaults = new VoiceSettings();
                settings = OverlayCatalog(defaults, entry.Settings);
            }

            var sdkVoiceSettings = new External.VoiceSettingsResponseModel(
                stability: settings.Stability,
                useSpeakerBoost: settings.UseSpeakerBoost,
                similarityBoost: settings.SimilarityBoost,
                style: settings.Style,
                speed: settings.Speed);

            return API.TextToSpeech.CreateTextToSpeechByVoiceIdAsync(
                voiceId: entry.Id,
                text: text,
                voiceSettings: sdkVoiceSettings,
                cancellationToken: cancellationToken);
        }

        private static VoiceSettings OverlayCatalog(VoiceSettings defaults, VoiceCatalogEntrySettings? catalog)
        {
            if (catalog is null)
                return defaults;

            return new VoiceSettings(
                Stability: catalog.Stability ?? defaults.Stability,
                SimilarityBoost: catalog.SimilarityBoost ?? defaults.SimilarityBoost,
                Style: catalog.Style ?? defaults.Style,
                UseSpeakerBoost: catalog.UseSpeakerBoost ?? defaults.UseSpeakerBoost,
                Speed: catalog.Speed ?? defaults.Speed);
        }
    }
}
