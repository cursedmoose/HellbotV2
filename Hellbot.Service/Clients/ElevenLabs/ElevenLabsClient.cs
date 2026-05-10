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

            VoiceSettings settings = userVoiceSettings ?? entry.Settings ?? new VoiceSettings();

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
    }
}
