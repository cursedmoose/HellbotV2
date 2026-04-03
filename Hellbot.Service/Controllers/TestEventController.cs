using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;
using Hellbot.Core.Events.Test;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class TestEventController(IEventBus bus) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> PingTest()
        {

            await bus.Publish(new TestEvent { Source = EventSource.API, Data = new TestPayload { Message = "OK" } });
            return Ok();
        }

        [HttpPost("test")]
        public async Task<IActionResult> Test(TestEvent evt)
        {
            await bus.Publish(evt);
            return Ok();
        }

        [HttpPost("chat")]
        public async Task<IActionResult> ChatMessage(ChatReceivedPayload evt)
        {
            var real_evt = new ChatReceivedEvent { Source = EventSource.API, Data = evt };
            // var real_evt = new ChatReceivedEvent("test", EventSource.API, evt.Message, evt.User); 
            await bus.Publish(real_evt);
            return Ok();
        }
    }
}
