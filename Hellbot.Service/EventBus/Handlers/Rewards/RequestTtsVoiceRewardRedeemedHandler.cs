using Hellbot.Core.Events;
using Hellbot.Core.Events.Rewards;
using Hellbot.Core.Events.Tts;
using Hellbot.Core.Tts;
using Hellbot.Service.EventBus.Handlers;

namespace Hellbot.Service.EventBus.Handlers.Rewards;

public sealed class RequestTtsVoiceRewardRedeemedHandler(IEventBus bus, ILogger<RequestTtsVoiceRewardRedeemedHandler> logger)
    : EventHandlerBase<RequestTtsVoiceRewardRedeemed>
{
    private const string RumorMaleVoiceKey = "voice/rumor-m";

    public override async Task Handle(RequestTtsVoiceRewardRedeemed evt)
    {
        var message = evt.Data.UserInput?.Trim();
        if (string.IsNullOrWhiteSpace(message))
        {
            logger.LogDebug(
                "Skipping RequestTtsVoice redemptionId={RedemptionId}. Empty user_input.",
                evt.Data.RedemptionId);
            return;
        }

        await bus.Publish(new EnqueueTts
        {
            Data = new TtsRequest
            {
                RequestId = evt.Id,
                Message = message,
                VoiceKey = RumorMaleVoiceKey,
                VoiceSettings = null,
                SceneId = null,
            },
            Source = EventSource.Internal with { Channel = nameof(RequestTtsVoiceRewardRedeemedHandler) },
            Context = evt.Context,
        });
    }
}
