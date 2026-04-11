using Hellbot.Core.Events.Chat;
using System.Threading.Channels;

namespace Hellbot.Service.Tts
{
    public class TtsQueue : ITtsQueue
    {
        private readonly Channel<TtsRequested> _channel =
            Channel.CreateUnbounded<TtsRequested>();

        public ValueTask EnqueueAsync(TtsRequested evt)
            => _channel.Writer.WriteAsync(evt);

        public IAsyncEnumerable<TtsRequested> DequeueAllAsync(CancellationToken ct)
            => _channel.Reader.ReadAllAsync(ct);

        public int Length() => _channel.Reader.Count;
    }
}