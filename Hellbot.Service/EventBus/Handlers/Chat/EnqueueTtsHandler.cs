using Hellbot.Core.Events.Tts;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Tts;

namespace Hellbot.Service.EventBus.Handlers.Chat;

public class EnqueueTtsHandler(ITtsQueue ttsQueue, ILogger<EnqueueTtsHandler> logger) : EventHandlerBase<EnqueueTts>
{
    public override async Task Handle(EnqueueTts evt)
    {
        await ttsQueue.EnqueueAsync(evt.Data);
        logger.LogInformation("Enqueued request={RequestId}. Queue length={Length}", evt.Data.RequestId, ttsQueue.Length());
    }
}
