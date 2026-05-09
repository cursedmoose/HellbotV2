using Hellbot.Core.Entitlements;
using Hellbot.Service.Entitlements;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers;

[Route("api/entitlements")]
[ApiController]
public class EntitlementCatalogController(IEntitlementService entitlements) : ControllerBase
{
    public record CreateEntitlementCatalogItemRequest
    {
        public required EntitlementType EntitlementType { get; init; }
        public required string EntitlementId { get; init; }
    }

    public record SetCatalogItemActiveRequest
    {
        public required bool IsActive { get; init; }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEntitlementCatalogItemRequest body)
    {
        var item = new EntitlementCatalogItem
        {
            Id = Guid.NewGuid(),
            EntitlementType = body.EntitlementType,
            EntitlementId = body.EntitlementId,
            IsActive = true,
        };

        var inserted = await entitlements.TryCreateCatalogItemAsync(item);
        if (inserted == CreateCatalogItemResult.DuplicateKey)
            return Conflict();

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpGet("type/{entitlementType}")]
    public async Task<ActionResult<IReadOnlyList<EntitlementCatalogItem>>> GetByType(EntitlementType entitlementType)
    {
        var rows = await entitlements.GetCatalogByTypeAsync(entitlementType);
        return Ok(rows);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EntitlementCatalogItem>> GetById(Guid id)
    {
        var row = await entitlements.GetCatalogByIdAsync(id);
        return row is null ? NotFound() : row;
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] SetCatalogItemActiveRequest body)
    {
        var updated = await entitlements.SetCatalogItemActiveAsync(id, body.IsActive);
        return updated == 0 ? NotFound() : NoContent();
    }
}
