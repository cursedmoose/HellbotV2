using Hellbot.Core.Events;
using Hellbot.Core.Events.Test;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestEventController(IEventBus bus) : EventController(bus)
    {
        [HttpGet]
        public Task<IActionResult> PingTest()
            => Publish(new TestEvent { Source = EventSource.API, Data = new TestPayload { Message = "OK" } });

        [HttpPost]
        public Task<IActionResult> Test(TestPayload evt)
            => Publish(new TestEvent { Source = EventSource.API, Data = evt });
    }
}
