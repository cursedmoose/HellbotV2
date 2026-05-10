using Hellbot.Core.Entitlements;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Preferences;
using Hellbot.Core.Events.Entitlements;
using Hellbot.Core.Users;
using Hellbot.Service.Users.Identity;
using Hellbot.Service.Entitlements;
using Hellbot.Service.Users;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    /// <summary>User entitlement grants + equipped preference slots (preference snapshot).</summary>
    [Route("api/users")]
    [ApiController]
    public class UsersController(IEventBus bus, IEntitlementService entitlements, IUserService userService)
        : EventPublishingController(bus)
    {
        public sealed record GrantRoleRequest
        {
            /// <summary>Prefer this: polymorphic JSON with <c>$kind</c>: <c>HellbotUser</c>, <c>PlatformAccount</c>, or <c>PlatformUsername</c>.</summary>
            public UserLocator? Recipient { get; init; }

            /// <summary>Fallback for existing clients: internal <c>users.id</c> when <see cref="Recipient"/> is omitted.</summary>
            public Guid? UserId { get; init; }

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
            public required UserPreferenceSnapshot PreferenceSnapshot { get; init; }
        }

        /// <summary>Granted catalog rows + resolved equipped selections.</summary>
        [HttpGet("entitlements")]
        [ProducesResponseType(typeof(UserCapabilitiesResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserCapabilitiesResponse>> GetCapabilities([FromQuery] UserIdentity recipient)
        {
            var user = await userService.GetOrCreateUserAsync(recipient);
            var snap = await entitlements.GetCapabilitiesAsync(user.Id);
            return Ok(new UserCapabilitiesResponse
            {
                UserId = snap.UserId,
                Entitlements = snap.Entitlements,
                PreferenceSnapshot = snap.PreferenceSnapshot,
            });
        }

        /// <summary>Grant a catalog entitlement to a user by publishing <see cref="GrantEntitlement"/>.</summary>
        /// <remarks>Endpoint: <c>POST /api/users/entitlements</c>.</remarks>
        [HttpPost("entitlements")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<IActionResult> GrantEntitlement([FromBody] GrantEntitlementPayload payload)
            => Publish(new GrantEntitlement { Source = EventSource.API, Data = payload });

        /// <summary>Set which granted catalog row is equipped for an <see cref="EntitlementType"/> slot.</summary>
        [HttpPut("preferences")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PutPreference([FromBody] UpsertUserPreferenceRequest body)
        {
            await PublishEvent(new SetUserPreference
            {
                Source = EventSource.API,
                Data = new SetUserPreferencePayload
                {
                    EntitlementType = body.EntitlementType,
                    SelectedEntitlementCatalogId = body.SelectedEntitlementCatalogId,
                },
            });
            return NoContent();
        }

        /// <summary>Upgrade role if currently below target. Use polymorphic <see cref="GrantRoleRequest.Recipient"/> or legacy <see cref="GrantRoleRequest.UserId"/>.</summary>
        [HttpPost("role")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> GrantRole([FromBody] GrantRoleRequest body)
        {
            if (body.Recipient is not null && body.UserId is Guid uidBoth && uidBoth != Guid.Empty)
                return BadRequest("Specify only one of Recipient or UserId.");

            UserLocator? locator = body.Recipient;
            if (locator is null && body.UserId is Guid uidFallback && uidFallback != Guid.Empty)
                locator = new UserLocator.HellbotUser(uidFallback);

            if (locator is null || body.Role == Role.None)
                return BadRequest();

            switch (await userService.ResolveAsync(locator))
            {
                case UserResolutionResult.NotFound:
                    return NotFound();

                case UserResolutionResult.AmbiguousUsername a:
                    return Conflict(new { message = "Multiple Hellbot users match this username.", candidates = a.CandidateHellbotUserIds });

                case UserResolutionResult.Resolved resolved:
                    var upgraded = await userService.TryUpgradeRoleAsync(resolved.HellbotUserId, body.Role);
                    return upgraded ? NoContent() : NotFound();

                default:
                    throw new InvalidOperationException($"{nameof(UserResolutionResult)} exhaustive switch failure.");
            }
        }

        /// <summary>Clear equipped selection for one slot (preferences row removed).</summary>
        [HttpDelete("preferences/{entitlementType}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeletePreference(EntitlementType entitlementType)
        {
            await PublishEvent(new DeleteUserPreference
            {
                Source = EventSource.API,
                Data = new DeleteUserPreferencePayload { EntitlementType = entitlementType },
            });
            return NoContent();
        }
    }
}
