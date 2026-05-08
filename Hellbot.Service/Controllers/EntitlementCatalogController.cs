using Hellbot.Core.Entitlements;
using Hellbot.Service.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace Hellbot.Service.Controllers;

[Route("api/entitlements")]
[ApiController]
public class EntitlementCatalogController(EntitlementCatalogTable catalog) : ControllerBase
{
    private const int SqliteConstraint = 19; // SQLITE_CONSTRAINT
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

        try
        {
            await catalog.Insert(item);
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == SqliteConstraint)
        {
            return Conflict();
        }

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpGet("type/{entitlementType}")]
    public async Task<ActionResult<IReadOnlyList<EntitlementCatalogItem>>> GetByType(EntitlementType entitlementType)
    {
        var rows = await catalog.GetByType(entitlementType);
        return Ok(rows);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EntitlementCatalogItem>> GetById(Guid id)
    {
        var row = await catalog.GetById(id);
        return row is null ? NotFound() : row;
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] SetCatalogItemActiveRequest body)
    {
        var updated = await catalog.SetIsActive(id, body.IsActive);
        return updated == 0 ? NotFound() : NoContent();
    }
}
