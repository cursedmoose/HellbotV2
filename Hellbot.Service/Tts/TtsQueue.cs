using System.Threading.Channels;

namespace Hellbot.Service.Tts
{
    public class TtsQueue : ITtsQueue
    {
        private readonly Channel<TtsRequest> _channel =
            Channel.CreateUnbounded<TtsRequest>();

        public ValueTask EnqueueAsync(TtsRequest evt)
            => _channel.Writer.WriteAsync(evt);

        public IAsyncEnumerable<TtsRequest> DequeueAllAsync(CancellationToken ct)
            => _channel.Reader.ReadAllAsync(ct);

        public int Length() => _channel.Reader.Count;
    }
}