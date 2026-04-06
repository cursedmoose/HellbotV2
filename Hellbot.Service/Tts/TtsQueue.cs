using Hellbot.Core.Events.Chat;
using System.Threading.Channels;

namespace Hellbot.Service.Tts
{
    public class TtsQueue : ITtsQueue
    {
        private readonly Channel<TtsRequestEvent> _channel =
            Channel.CreateUnbounded<TtsRequestEvent>();

        public ValueTask EnqueueAsync(TtsRequestEvent evt)
            => _channel.Writer.WriteAsync(evt);

        public IAsyncEnumerable<TtsRequestEvent> DequeueAllAsync(CancellationToken ct)
            => _channel.Reader.ReadAllAsync(ct);

        public int Length() => _channel.Reader.Count;
    }
}