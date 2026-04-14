using Items.Application.Common;
using Items.Application.ItemClasses;
using Items.Application.ItemClasses.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Items.API.Controllers;

/// <summary>Behavioral templates inherited by items.</summary>
[ApiController]
[Route("api/v1/item-classes")]
[Produces("application/json")]
[Tags("Item Classes")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ItemClassesController : ControllerBase
{
    private readonly IItemClassService _service;

    public ItemClassesController(IItemClassService service)
    {
        _service = service;
    }

    /// <summary>Returns a paginated list of active item classes.</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of records per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of item class summaries ordered by code.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ItemClassSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>Returns a single item class by ID.</summary>
    /// <param name="id">The item class ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The full item class record.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ItemClassDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(dto);
    }

    /// <summary>Creates a new item class.</summary>
    /// <param name="request">The item class to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created item class.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ItemClassDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateItemClassRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Updates an existing item class (ItemNature is non-editable post-create).</summary>
    /// <param name="id">The item class ID to update.</param>
    /// <param name="request">The updated item class values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated item class.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ItemClassDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateItemClassRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(dto);
    }

    /// <summary>Soft-deletes an item class.</summary>
    /// <param name="id">The item class ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Clones an item class with a new code and name.</summary>
    /// <param name="id">The source item class ID.</param>
    /// <param name="request">The new code and name for the clone.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created item class clone.</returns>
    [HttpPost("{id:int}/clone")]
    [ProducesResponseType(typeof(ItemClassDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Clone(int id, [FromBody] CloneItemClassRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.CloneAsync(id, request.Code, request.Name, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
