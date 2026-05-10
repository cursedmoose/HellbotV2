using Hellbot.Core.Tts;

namespace Hellbot.Service.Tts
{
    public interface ITtsQueue
    {
        ValueTask EnqueueAsync(TtsRequest evt);
        IAsyncEnumerable<TtsRequest> DequeueAllAsync(CancellationToken ct);
        int Length();
    }
}
