using Hellbot.Core.Events;
using Hellbot.Core.Events.Session;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [ApiController]
    [Route("stream")]
    public class StreamEventsController(IEventBus bus): EventController(bus)
    {
        [HttpPost("start")]
        public Task<IActionResult> StartStream(StreamStartPayload evt)
            => Publish(new StreamStartEvent { Source = EventSource.API, Data = evt });

        [HttpPost("stop")]
        public Task<IActionResult> StopStream(StreamStopPayload evt)
            => Publish(new StreamStopEvent { Source = EventSource.API, Data = evt });
    }
}
