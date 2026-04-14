using Items.Application.Common;
using Items.Application.PriceLists;
using Items.Application.PriceLists.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Items.API.Controllers;

/// <summary>Sales and purchase price list headers with pricing slabs.</summary>
[ApiController]
[Route("api/v1/price-lists")]
[Produces("application/json")]
[Tags("Price Lists")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class PriceListsController : ControllerBase
{
    private readonly IPriceListService _service;

    public PriceListsController(IPriceListService service)
    {
        _service = service;
    }

    /// <summary>Returns a paginated list of price lists ordered by priority (highest first).</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of records per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of price list summaries.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PriceListSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>Returns a price list with all pricing slabs.</summary>
    /// <param name="id">The price list ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The price list with line items sorted by MinQty.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PriceListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(dto);
    }

    /// <summary>Creates a price list with pricing slabs.</summary>
    /// <param name="request">The price list to create including line items.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created price list.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PriceListDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreatePriceListRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Updates a price list (ApplicableTo is non-editable post-create).</summary>
    /// <param name="id">The price list ID to update.</param>
    /// <param name="request">The updated values. Line items collection is fully replaced.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated price list.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PriceListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePriceListRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(dto);
    }

    /// <summary>Soft-deletes a price list (blocked if linked to an active SO or contract).</summary>
    /// <param name="id">The price list ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Clones a price list as Draft with a new name.</summary>
    /// <param name="id">The source price list ID.</param>
    /// <param name="request">The new name for the clone.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created price list clone in Draft status.</returns>
    [HttpPost("{id:int}/clone")]
    [ProducesResponseType(typeof(PriceListDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Clone(int id, [FromBody] ClonePriceListRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.CloneAsync(id, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
