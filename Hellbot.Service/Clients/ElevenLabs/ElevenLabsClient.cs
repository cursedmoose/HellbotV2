using External = ElevenLabs;
using Hellbot.Service.Config;
using Microsoft.Extensions.Options;

namespace Hellbot.Service.Clients.ElevenLabs
{
    public class ElevenLabsClient(IOptions<ElevenLabsOptions> options)
    {
        public readonly External.ElevenLabsClient API = new(options.Value.ApiKey);
        
        public async Task<byte[]> GenerateTts(string VoiceId, string Text)
        {
            return await API.TextToSpeech.CreateTextToSpeechByVoiceIdAsync(VoiceId, Text);
        }
    }
}
