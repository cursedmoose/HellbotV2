using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    public class ChatEventsController(IEventBus bus) : EventController(bus)
    {
        [HttpPost("chat/send")]
        public Task<IActionResult> SendChatMessage(SendChatPayload evt)
            => Publish(new SendChatMessage { Data = evt, Source = EventSource.API });

        [HttpPost("chat/receive")]
        public Task<IActionResult> ReceiveChatMessage(ChatReceivedPayload evt)
            => Publish(new ChatMessageReceived { Data = evt, Source = EventSource.API });
    }
}
