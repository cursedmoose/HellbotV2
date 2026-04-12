using Hellbot.Core.Events.Chat;
using Hellbot.Service.Tts;

namespace Hellbot.Service.EventBus.Handlers.Chat
{
    public class TtsRequestHandler(ITtsQueue ttsQueue, ILogger<TtsRequestHandler> logger) : EventHandlerBase<TtsRequested>
    {
        public override async Task Handle(TtsRequested evt)
        {
            await ttsQueue.EnqueueAsync(evt);
            logger.LogInformation("Enqueued request={RequestId}. Queue length={Length}", evt.Id, ttsQueue.Length());
        }
    }
}
