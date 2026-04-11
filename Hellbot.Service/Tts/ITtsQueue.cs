using Hellbot.Core.Events.Chat;

namespace Hellbot.Service.Tts
{
    public interface ITtsQueue
    {
        ValueTask EnqueueAsync(TtsRequested evt);
        IAsyncEnumerable<TtsRequested> DequeueAllAsync(CancellationToken ct);
        int Length();
    }
}
