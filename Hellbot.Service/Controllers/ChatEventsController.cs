using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [ApiController]
    public class ChatEventsController(IEventBus bus) : EventController(bus)
    {
        [HttpPost("chat/send")]
        public async Task<IActionResult> SendChatMessage(
            SendChatPayload evt,
            [FromQuery] Guid? asHellbotUserId = null,
            [FromQuery] string? asTwitchLogin = null)
        {
            var hellbotEvt = new SendChatMessage { Data = evt, Source = EventSource.API };
            var rejection = SeedUserContext(hellbotEvt, asHellbotUserId, asTwitchLogin);
            return rejection ?? await Publish(hellbotEvt);
        }

        [HttpPost("chat/receive")]
        public async Task<IActionResult> ReceiveChatMessage(
            ChatReceivedPayload evt,
            [FromQuery] Guid? asHellbotUserId = null,
            [FromQuery] string? asTwitchLogin = null)
        {
            var hellbotEvt = new ChatMessageReceived { Data = evt, Source = EventSource.API };
            var rejection = SeedUserContext(hellbotEvt, asHellbotUserId, asTwitchLogin);
            return rejection ?? await Publish(hellbotEvt);
        }

        [HttpPost("tts")]
        public async Task<IActionResult> TtsMessage(
            TtsRequestPayload evt,
            [FromQuery] Guid? asHellbotUserId = null,
            [FromQuery] string? asTwitchLogin = null)
        {
            var hellbotEvt = new TtsRequested { Data = evt, Source = EventSource.API };
            var rejection = SeedUserContext(hellbotEvt, asHellbotUserId, asTwitchLogin);
            return rejection ?? await Publish(hellbotEvt);
        }
    }
}
