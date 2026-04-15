using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;
using Hellbot.Core.TTS;
using Hellbot.Core.Users;
using Hellbot.Service.Tts;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Chat
{
    public class TtsRequestHandler(ITtsQueue ttsQueue, IUserService users, ILogger<TtsRequestHandler> logger) : EventHandlerBase<TtsRequested>
    {
        public override async Task Handle(TtsRequested evt)
        {
            var userId = evt.Context.User is UserContext { Info: User user }
                ? user.Id
                : Guid.Empty;
            var customizations = await users.GetUserCustomizations(userId);

            if (string.IsNullOrEmpty(customizations.VoiceId))
            {
                logger.LogDebug("Skipping TTS request={RequestId} User={UserId}. No voice configured.", evt.Id, userId);
                return;
            }

            var ttsRequest = new TtsRequest
            {
                RequestId = evt.Id,
                Message = evt.Data.Text,
                VoiceId = customizations.VoiceId,
                VoiceSettings = customizations.VoiceSettings ?? new(),
                SceneId = customizations.SceneId
            };

            await ttsQueue.EnqueueAsync(ttsRequest);
            logger.LogInformation("Enqueued request={RequestId}. Queue length={Length}", evt.Id, ttsQueue.Length());
        }
    }
}
