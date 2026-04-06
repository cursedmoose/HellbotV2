using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [ApiController]
    public class ChatEventsController(IEventBus bus) : EventController(bus)
    {
        [HttpPost("chat")]
        public Task<IActionResult> ChatMessage(ChatReceivedPayload evt)
            => Publish(new ChatReceivedEvent
            {
                Data = evt,
                Source = EventSource.API
            });

        [HttpPost("tts")]
        public Task<IActionResult> TtsMessage(TtsRequestPayload evt)
            => Publish(new TtsRequestEvent
            {
                Data = evt,
                Source = EventSource.API
            });
    }
}
