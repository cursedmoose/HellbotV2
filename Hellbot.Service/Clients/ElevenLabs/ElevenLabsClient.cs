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
            string VoiceId,
            string Text,
            VoiceSettings VoiceSettings,
            CancellationToken cancellationToken = default)
        {
            var sdkVoiceSettings = new External.VoiceSettingsResponseModel(
                stability: VoiceSettings.Stability,
                useSpeakerBoost: VoiceSettings.UseSpeakerBoost,
                similarityBoost: VoiceSettings.SimilarityBoost,
                style: VoiceSettings.Style,
                speed: VoiceSettings.Speed);

            return API.TextToSpeech.CreateTextToSpeechByVoiceIdAsync(
                voiceId: VoiceId,
                text: Text,
                voiceSettings: sdkVoiceSettings,
                cancellationToken: cancellationToken);
        }
    }
}
