using Items.Application.Common;
using Items.Application.Items;
using Items.Application.Items.Dtos;
using Items.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Items.API.Controllers;

/// <summary>Item master with sub-entities (alternate UOMs, specs, drawings, warehouse thresholds).</summary>
[ApiController]
[Route("api/v1/items")]
[Produces("application/json")]
[Tags("Items")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ItemsController : ControllerBase
{
    private readonly IItemService _service;

    public ItemsController(IItemService service)
    {
        _service = service;
    }

    /// <summary>Returns a paginated list of items; filterable by class, type, and nature.</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of records per page.</param>
    /// <param name="itemClassId">Filter by item class ID.</param>
    /// <param name="itemType">Filter by item type enum value.</param>
    /// <param name="itemNature">Filter by item nature (Goods or Service).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of item summaries.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ItemSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? itemClassId = null,
        [FromQuery] ItemType? itemType = null,
        [FromQuery] ItemNature? itemNature = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAllAsync(page, pageSize, itemClassId, itemType, itemNature, cancellationToken);
        return Ok(result);
    }

    /// <summary>Returns a full item record including all sub-entities.</summary>
    /// <param name="id">The item ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The full item with alternate UOMs, specs, drawings, and warehouse thresholds.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(dto);
    }

    /// <summary>Creates a new item with all sub-entities.</summary>
    /// <param name="request">The item to create including alternate UOMs, specs, drawings, and thresholds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created item.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateItemRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Updates an item (ItemNature and dimension flags are non-editable post-create).</summary>
    /// <param name="id">The item ID to update.</param>
    /// <param name="request">The updated item values. Sub-entity collections are fully replaced.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated item.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateItemRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(dto);
    }

    /// <summary>Soft-deletes an item (blocked if transaction history exists).</summary>
    /// <param name="id">The item ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Deep-clones an item including all sub-entities.</summary>
    /// <param name="id">The source item ID.</param>
    /// <param name="request">The new name and SKU for the clone.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created item clone.</returns>
    [HttpPost("{id:int}/clone")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Clone(int id, [FromBody] CloneItemRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.CloneAsync(id, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
