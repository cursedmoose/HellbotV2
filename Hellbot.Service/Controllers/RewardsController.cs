using Hellbot.Core.Entitlements;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Rewards;
using Hellbot.Core.Users;
using Hellbot.Service.Data.Tables.Users;
using Hellbot.Service.Users;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers;

[Route("api/rewards")]
[ApiController]
public class RewardsController(IEventBus bus, IUserService users, UserEntitlementsTable entitlements) : EventController(bus)
{
    [HttpPost]
    public Task<IActionResult> GrantReward(GrantRewardPayload evt)
        => Publish(new GrantReward { Source = EventSource.API, Data = evt });

    [HttpGet]
    public async Task<IReadOnlyList<UserEntitlement>> GetRewards(UserIdentity id)
    {
        var user = await users.GetOrCreateUser(id);
        return await entitlements.GetAll(user.Id);
    }
}
