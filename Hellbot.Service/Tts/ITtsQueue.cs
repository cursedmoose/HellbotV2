using Hellbot.Core.Events.Chat;

namespace Hellbot.Service.Tts
{
    public interface ITtsQueue
    {
        ValueTask EnqueueAsync(TtsRequestEvent evt);
        IAsyncEnumerable<TtsRequestEvent> DequeueAllAsync(CancellationToken ct);
        int Length();
    }
}
