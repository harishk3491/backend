using Items.Application.Common;
using Items.Application.ItemVendorMappings;
using Items.Application.ItemVendorMappings.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Items.API.Controllers;

/// <summary>Vendor-item purchase relationships including UOM grid and pricing slabs.</summary>
[ApiController]
[Route("api/v1/item-vendor-mappings")]
[Produces("application/json")]
[Tags("Item Vendor Mappings")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ItemVendorMappingsController : ControllerBase
{
    private readonly IItemVendorMappingService _service;

    public ItemVendorMappingsController(IItemVendorMappingService service)
    {
        _service = service;
    }

    /// <summary>Returns a paginated list of vendor mappings; filterable by itemId and vendorId.</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of records per page.</param>
    /// <param name="itemId">Filter by item ID.</param>
    /// <param name="vendorId">Filter by vendor ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of vendor mapping summaries.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ItemVendorMappingSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? itemId = null,
        [FromQuery] int? vendorId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAllAsync(page, pageSize, itemId, vendorId, cancellationToken);
        return Ok(result);
    }

    /// <summary>Returns a full vendor mapping with purchase UOM grid and pricing slabs.</summary>
    /// <param name="id">The vendor mapping ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The full vendor mapping record.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ItemVendorMappingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(dto);
    }

    /// <summary>Creates a vendor-item mapping with purchase UOM grid and pricing.</summary>
    /// <param name="request">The vendor mapping to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created vendor mapping.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ItemVendorMappingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateItemVendorMappingRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Updates a vendor mapping (ItemId and VendorId are non-editable post-create).</summary>
    /// <param name="id">The vendor mapping ID to update.</param>
    /// <param name="request">The updated values. UOM grid and pricing collections are fully replaced.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated vendor mapping.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ItemVendorMappingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateItemVendorMappingRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(dto);
    }

    /// <summary>Soft-deletes a vendor mapping (blocked if referenced in an active PO).</summary>
    /// <param name="id">The vendor mapping ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Clones a vendor mapping, optionally targeting a different item or vendor.</summary>
    /// <param name="id">The source vendor mapping ID.</param>
    /// <param name="request">Optional override for target item or vendor; clone is never preferred by default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created vendor mapping clone.</returns>
    [HttpPost("{id:int}/clone")]
    [ProducesResponseType(typeof(ItemVendorMappingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Clone(int id, [FromBody] CloneItemVendorMappingRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.CloneAsync(id, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
