using Items.Application.Uoms;
using Items.Application.Uoms.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Items.API.Controllers;

/// <summary>Unit of measure lookup master.</summary>
[ApiController]
[Route("api/v1/uoms")]
[Produces("application/json")]
[Tags("Units of Measure")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class UomsController : ControllerBase
{
    private readonly IUomService _service;

    public UomsController(IUomService service)
    {
        _service = service;
    }

    /// <summary>Returns all active UOMs.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of active UOMs ordered by code.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<UomDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var dtos = await _service.GetAllAsync(cancellationToken);
        return Ok(dtos);
    }

    /// <summary>Returns a single UOM by ID.</summary>
    /// <param name="id">The UOM ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested UOM.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(dto);
    }

    /// <summary>Creates a new UOM.</summary>
    /// <param name="request">The UOM to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created UOM.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UomDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUomRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Updates an existing UOM.</summary>
    /// <param name="id">The UOM ID to update.</param>
    /// <param name="request">The updated UOM values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated UOM.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUomRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(dto);
    }

    /// <summary>Soft-deletes a UOM (blocked if used on any item).</summary>
    /// <param name="id">The UOM ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
