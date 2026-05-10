using Hellbot.Core.Entitlements;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Context;
using Hellbot.Core.Events.Tts;
using Hellbot.Core.Tts;
using Hellbot.Core.Users;

namespace Hellbot.Service.EventBus.Handlers.Chat;

public class TtsRequestHandler(IEventBus bus, ILogger<TtsRequestHandler> logger) : EventHandlerBase<TtsRequested>
{
    public override async Task Handle(TtsRequested evt)
    {
        if (evt.Context.User is not UserContext uc)
        {
            logger.LogDebug("Skipping TTS request={RequestId}. No enriched user context.", evt.Id);
            return;
        }

        var user = uc.Info!;
        var preferenceSnapshot = uc.PreferenceSnapshot ?? UserPreferenceSnapshot.Empty;
        var voiceItem = preferenceSnapshot.GetOrDefault(EntitlementType.TtsVoice);
        var voiceKey = voiceItem?.EntitlementId;

        if (string.IsNullOrEmpty(voiceKey))
        {
            logger.LogDebug(
                "Skipping TTS request={RequestId} User={UserId}. No TtsVoice equipped.",
                evt.Id,
                user.Id);
            return;
        }

        var avatarItem = preferenceSnapshot.GetOrDefault(EntitlementType.TtsAvatar);
        var sceneCandidate = avatarItem?.EntitlementId;
        var ttsRequest = new TtsRequest
        {
            RequestId = evt.Id,
            Message = evt.Data.Text,
            VoiceKey = voiceKey,
            VoiceSettings = null,
            SceneId = string.IsNullOrEmpty(sceneCandidate) ? null : sceneCandidate,
        };

        await bus.Publish(new EnqueueTts
        {
            Data = ttsRequest,
            Source = EventSource.Internal with { Channel = "TtsRequestHandler" },
            Context = evt.Context,
        });
    }
}
