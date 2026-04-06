using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;
using Hellbot.Service.Tts;

namespace Hellbot.Service.EventBus.Handlers.Chat
{
    public class TtsRequestHandler(ITtsQueue ttsQueue, ILogger<TtsRequestHandler> logger) : IEventHandler
    {
        public void Register(IEventBus bus)
        {
            bus.Subscribe<TtsRequestEvent>(Handle);
        }

        private async Task Handle(TtsRequestEvent evt)
        {
            await ttsQueue.EnqueueAsync(evt);
            logger.LogInformation("Enqueued request={RequestId}. Queue length={Length}", evt.Id, ttsQueue.Length());
        }
    }
}
