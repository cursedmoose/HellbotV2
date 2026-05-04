using Hellbot.Core.Events;
using Hellbot.Core.Events.Audio;
using Hellbot.Service.Audio;
using Hellbot.Service.Clients.Whisper;
using NAudio.Wave;

namespace Hellbot.Service.EventBus.Handlers.Audio
{
    public class SpeechToTextHandler(
        WhisperClient whisper,
        IEventBus bus,
        ILogger<SpeechToTextHandler> logger) : EventHandlerBase<VoiceSegmentCaptured>
    {
        public async override Task Handle(VoiceSegmentCaptured evt)
        {
            var wavStream = AudioConverter.ToWavStream(evt.Data.RawAudio);

            var text = await whisper.TranscribeAsync(wavStream);

            logger.LogInformation("Text transcribed: {Text}", text);
            if (text == "[BLANK_AUDIO]" || text is null)
            {
                return;
            }

            await bus.Publish(new VoiceTranscriptionCompleted
            {
                Data = new()
                {
                    Start = evt.Data.Start,
                    End = evt.Data.End,
                    Text = text
                },
                Source = EventSource.Internal with { Channel = "SpeechToTextHandler" }
            });
        }
    }
}
