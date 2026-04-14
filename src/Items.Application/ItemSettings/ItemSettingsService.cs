using AutoMapper;
using Items.Application.Common.Interfaces;
using Items.Application.ItemSettings.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Items.Application.ItemSettings;

public class ItemSettingsService : IItemSettingsService
{
    private readonly IItemsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ItemSettingsService> _logger;

    public ItemSettingsService(IItemsDbContext context, IMapper mapper, ILogger<ItemSettingsService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ItemSettingsDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _context.ItemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (settings is null)
        {
            _logger.LogInformation("ItemSettings not found; returning defaults");
            return new ItemSettingsDto { ItemCodeMaxLength = 20 };
        }

        return _mapper.Map<ItemSettingsDto>(settings);
    }

    public async Task<ItemSettingsDto> UpsertAsync(
        UpdateItemSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var settings = await _context.ItemSettings
            .FirstOrDefaultAsync(cancellationToken);

        if (settings is null)
        {
            settings = new Domain.Entities.ItemSettings();
            await _context.ItemSettings.AddAsync(settings, cancellationToken);
            _logger.LogInformation("Creating ItemSettings singleton");
        }
        else
        {
            _logger.LogInformation("Updating ItemSettings singleton (Id={Id})", settings.Id);
        }

        _mapper.Map(request, settings);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ItemSettingsDto>(settings);
    }
}
