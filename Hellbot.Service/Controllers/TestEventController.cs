using Hellbot.Core.Events;
using Hellbot.Core.Events.Test;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [Route("api/events/test")]
    [ApiController]
    public class TestEventController(IEventBus bus) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> PingTest()
        {

            await bus.Publish(new TestMessageEvent() { Message = "ok" });
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> PostTest(TestMessageEvent evt)
        {
            await bus.Publish(evt);
            return Ok();
        }
    }
}
