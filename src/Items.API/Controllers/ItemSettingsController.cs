using Items.Application.ItemSettings;
using Items.Application.ItemSettings.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Items.API.Controllers;

/// <summary>Global configuration singleton for the item module.</summary>
[ApiController]
[Route("api/v1/item-settings")]
[Produces("application/json")]
[Tags("Item Settings")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ItemSettingsController : ControllerBase
{
    private readonly IItemSettingsService _service;

    public ItemSettingsController(IItemSettingsService service)
    {
        _service = service;
    }

    /// <summary>Returns the singleton item settings record (defaults if never saved).</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current item settings.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ItemSettingsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var dto = await _service.GetAsync(cancellationToken);
        return Ok(dto);
    }

    /// <summary>Creates or updates the singleton item settings.</summary>
    /// <param name="request">The settings values to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated item settings.</returns>
    [HttpPut]
    [ProducesResponseType(typeof(ItemSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upsert(
        [FromBody] UpdateItemSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await _service.UpsertAsync(request, cancellationToken);
        return Ok(dto);
    }
}
