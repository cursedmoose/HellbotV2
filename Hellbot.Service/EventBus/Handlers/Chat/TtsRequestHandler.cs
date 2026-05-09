using Hellbot.Core.Entitlements;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;
using Hellbot.Core.TTS;
using Hellbot.Core.Users;
using Hellbot.Service.Tts;

namespace Hellbot.Service.EventBus.Handlers.Chat;

public class TtsRequestHandler(ITtsQueue ttsQueue, ILogger<TtsRequestHandler> logger) : EventHandlerBase<TtsRequested>
{
    public override async Task Handle(TtsRequested evt)
    {
        if (evt.Context.EnrichedUserContext is not {} uc)
        {
            logger.LogDebug("Skipping TTS request={RequestId}. No enriched user context.", evt.Id);
            return;
        }

        var user = uc.Info!;
        var preferenceSnapshot = uc.PreferenceSnapshot ?? UserPreferenceSnapshot.Empty;
        var voiceItem = preferenceSnapshot.GetOrDefault(EntitlementType.TtsVoice);
        var voiceId = voiceItem?.EntitlementId;

        if (string.IsNullOrEmpty(voiceId))
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
            VoiceId = voiceId,
            VoiceSettings = new VoiceSettings(),
            SceneId = string.IsNullOrEmpty(sceneCandidate) ? null : sceneCandidate,
        };

        await ttsQueue.EnqueueAsync(ttsRequest);
        logger.LogInformation("Enqueued request={RequestId}. Queue length={Length}", evt.Id, ttsQueue.Length());
    }
}
