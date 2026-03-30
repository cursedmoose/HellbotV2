using Hellbot.Core.Events;
using Hellbot.Core.Events.Test;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class TestEventController : ControllerBase
    {
        private readonly IEventBus _bus;

        public TestEventController(IEventBus bus)
        {
            _bus = bus;
        }

        [HttpGet("test")]
        public async Task<IActionResult> PingTest()
        {

            await _bus.PublishAsync(new TestMessageEvent() { Message = "ok" });
            return Ok();
        }

        [HttpPost("test")]
        public async Task<IActionResult> PostTest(TestMessageEvent evt)
        {
            await _bus.PublishAsync(evt);
            return Ok();
        }
    }
}
