using Hellbot.Core.Events;
using Hellbot.Service.Controllers.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [IncludeUserContext]
    [ApiController]
    public abstract class EventPublishingController(IEventBus bus) : ControllerBase
    {
        protected async Task PublishEvent(IHellbotEvent evt)
        {
            UserContextSeeder.ApplyPendingToEvent(HttpContext, evt);
            await bus.Publish(evt);
        }

        protected async Task<IActionResult> Publish(IHellbotEvent evt)
        {
            await PublishEvent(evt);
            return Ok();
        }
    }

    [Route("api/events")]
    public abstract class EventController(IEventBus bus) : EventPublishingController(bus);
}
