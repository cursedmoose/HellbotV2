using Hellbot.Core.Events;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [Route("api/events")]
    [ApiController]
    public abstract class EventController(IEventBus bus) : ControllerBase
    {
        protected async Task<IActionResult> Publish(IHellbotEvent evt)
        {
            await bus.Publish(evt);
            return Ok();
        }
    }
}
