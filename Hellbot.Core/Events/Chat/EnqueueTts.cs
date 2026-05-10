using Hellbot.Core.TTS;

namespace Hellbot.Core.Events.Chat
{
    public record EnqueueTts : HellbotEvent<TtsRequest>;
}
