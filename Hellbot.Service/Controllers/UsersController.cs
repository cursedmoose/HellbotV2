using Hellbot.Core.Entitlements;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Preferences;
using Hellbot.Core.Events.Entitlements;
using Hellbot.Core.Users;
using Hellbot.Service.Entitlements;
using Hellbot.Service.Users;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers;

/// <summary>User entitlement grants + equipped preference slots (experience snapshot).</summary>
[Route("api/users")]
[ApiController]
public class UsersController(IEventBus bus, IEntitlementService entitlements, IUserService userService) : ControllerBase
{
    public sealed record GrantRoleRequest
    {
        /// <summary>Internal <c>users.id</c> (not platform account id).</summary>
        public required Guid UserId { get; init; }
        public required Role Role { get; init; }
    }
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
        var snap = await entitlements.GetCapabilitiesAsync(recipient);
        return Ok(new UserCapabilitiesResponse
        {
            UserId = snap.UserId,
            Entitlements = snap.Entitlements,
            Experience = snap.Experience,
        });
    }

    /// <summary>Grant a catalog entitlement to a user by publishing <see cref="GrantEntitlement"/>.</summary>
    /// <remarks>Endpoint: <c>POST /api/users/entitlements</c>.</remarks>
    [HttpPost("entitlements")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GrantEntitlement([FromBody] GrantEntitlementPayload payload)
    {
        await bus.Publish(new GrantEntitlement { Source = EventSource.API, Data = payload });
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

    /// <summary>Upgrade a user's <see cref="Role"/> by internal user id if below the target.</summary>
    [HttpPost("role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GrantRole([FromBody] GrantRoleRequest body)
    {
        if (body.UserId == Guid.Empty || body.Role == Role.None)
            return BadRequest();

        var updated = await userService.UpdateUserRoleForUserAsync(body.UserId, body.Role);
        if (!updated)
            return NotFound();

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
