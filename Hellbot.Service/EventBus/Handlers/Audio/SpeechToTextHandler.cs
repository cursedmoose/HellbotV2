using Hellbot.Core.Events.Audio;
using Hellbot.Service.Audio;
using Hellbot.Service.Clients.Whisper;
using NAudio.Wave;

namespace Hellbot.Service.EventBus.Handlers.Audio
{
    public class SpeechToTextHandler(WhisperClient whisper, ILogger<SpeechToTextHandler> logger) : EventHandlerBase<VoiceSegmentCaptured>
    {
        public async override Task Handle(VoiceSegmentCaptured evt)
        {
            var wavStream = AudioConverter.ToWavStream(evt.Data.RawAudio);

            var text = await whisper.TranscribeAsync(wavStream);

            logger.LogInformation("Text transcribed: {Text}", text);
        }
    }
}
