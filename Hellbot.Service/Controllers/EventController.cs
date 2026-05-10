using Hellbot.Core.Events;
using Hellbot.Core.Users;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [Route("api/events")]
    [ApiController]
    public abstract class EventController(IEventBus bus) : ControllerBase
    {
        protected IActionResult? SeedUserContext(
            IHellbotEvent evt,
            Guid? asHellbotUserId,
            string? asTwitchLogin)
        {
            var twitchLogin = string.IsNullOrWhiteSpace(asTwitchLogin)
                ? null
                : asTwitchLogin.Trim();
            var hasHellbotUserId = asHellbotUserId.HasValue;
            var hasTwitchLogin = twitchLogin is not null;

            if (hasHellbotUserId && hasTwitchLogin)
                return BadRequest("Specify only one of asHellbotUserId or asTwitchLogin.");

            if (hasHellbotUserId)
                evt.Context = EventContext.From(new UserLocator.HellbotUser(asHellbotUserId!.Value));
            else if (hasTwitchLogin)
                evt.Context = EventContext.From(
                    new UserLocator.PlatformUsername(PlatformSource.Twitch, twitchLogin!));

            return null;
        }

        protected async Task<IActionResult> Publish(IHellbotEvent evt)
        {
            await bus.Publish(evt);
            return Ok();
        }
    }
}
