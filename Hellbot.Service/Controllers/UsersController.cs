using Hellbot.Core.Entitlements;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Preferences;
using Hellbot.Core.Events.Rewards;
using Hellbot.Core.Users;
using Hellbot.Service.Data.Tables.Users;
using Hellbot.Service.Users;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers;

/// <summary>User entitlement grants + equipped preference slots (experience snapshot).</summary>
[Route("api/users")]
[ApiController]
public class UsersController(
    IEventBus bus,
    IUserService users,
    UserPreferencesTable preferencesTable,
    UserEntitlementsTable entitlements) : ControllerBase
{
    public sealed record UpsertUserPreferenceRequest
    {
        public required EntitlementType EntitlementType { get; init; }
        public required Guid SelectedEntitlementCatalogId { get; init; }
    }

    public sealed record UserCapabilitiesResponse
    {
        public required Guid UserId { get; init; }
        public required IReadOnlyList<UserEntitlement> Entitlements { get; init; }
        public required UserExperienceSnapshot Experience { get; init; }
    }

    /// <summary>Granted catalog rows + resolved equipped selections.</summary>
    [HttpGet("entitlements")]
    [ProducesResponseType(typeof(UserCapabilitiesResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserCapabilitiesResponse>> GetCapabilities([FromQuery] UserIdentity recipient)
    {
        var user = await users.GetOrCreateUser(recipient);
        var granted = await entitlements.GetAll(user.Id);
        var experience = await preferencesTable.ResolveExperienceAsync(user.Id);

        return Ok(new UserCapabilitiesResponse
        {
            UserId = user.Id,
            Entitlements = granted,
            Experience = experience,
        });
    }

    /// <summary>Grant a catalog entitlement to a user via the reward pipeline (<see cref="GrantReward"/>).</summary>
    /// <remarks>Same behavior as POST <c>/api/rewards</c>; use whichever route suits your caller.</remarks>
    [HttpPost("entitlements")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GrantEntitlement([FromBody] GrantRewardPayload payload)
    {
        await bus.Publish(new GrantReward { Source = EventSource.API, Data = payload });
        return Ok();
    }

    /// <summary>Set which granted catalog row is equipped for an <see cref="EntitlementType"/> slot.</summary>
    [HttpPut("preferences")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PutPreference([FromQuery] UserIdentity recipient, [FromBody] UpsertUserPreferenceRequest body)
    {
        await bus.Publish(new SetUserPreference
        {
            Source = EventSource.API,
            Data = new SetUserPreferencePayload
            {
                Recipient = recipient,
                EntitlementType = body.EntitlementType,
                SelectedEntitlementCatalogId = body.SelectedEntitlementCatalogId,
            },
        });
        return NoContent();
    }

    /// <summary>Clear equipped selection for one slot (preferences row removed).</summary>
    [HttpDelete("preferences/{entitlementType}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeletePreference([FromQuery] UserIdentity recipient, EntitlementType entitlementType)
    {
        await bus.Publish(new DeleteUserPreference
        {
            Source = EventSource.API,
            Data = new DeleteUserPreferencePayload { Recipient = recipient, EntitlementType = entitlementType },
        });
        return NoContent();
    }
}
