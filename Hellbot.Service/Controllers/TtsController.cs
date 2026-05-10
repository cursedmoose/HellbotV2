using Hellbot.Core.Events;
using Hellbot.Core.Events.Tts;
using Hellbot.Core.Tts;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [Route("tts")]
    public class TtsController(IEventBus bus) : EventController(bus)
    {
        [HttpPost("request")]
        public Task<IActionResult> TtsMessage(TtsRequestPayload evt)
            => Publish(new TtsRequested { Data = evt, Source = EventSource.API });

        [HttpPost("enqueue")]
        public Task<IActionResult> Enqueue(TtsRequest body)
            => Publish(new EnqueueTts { Data = body, Source = EventSource.API });
    }
}
