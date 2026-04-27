using Hellbot.Core.Events.Audio;
using Hellbot.Service.Tts;

namespace Hellbot.Service.EventBus.Handlers.Audio
{
    public class MicSpeechEndedHandler(ITtsPlaybackGate ttsGate, ILogger<MicSpeechEndedHandler> logger) : EventHandlerBase<SpeechEnded>
    {
        public override Task Handle(SpeechEnded evt)
        {
            ttsGate.Resume("mic");
            return Task.CompletedTask;
        }
    }
}
