using Hellbot.Core.Events.Audio;
using Hellbot.Service.Tts;

namespace Hellbot.Service.EventBus.Handlers.Audio
{
    public class MicSpeechStartedHandler(ITtsPlaybackGate ttsGate, ILogger<MicSpeechStartedHandler> logger) : EventHandlerBase<SpeechStarted>
    {
        public override Task Handle(SpeechStarted evt)
        {
            ttsGate.Pause("mic");
            return Task.CompletedTask;
        }
    }
}
