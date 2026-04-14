using AutoMapper;
using AutoMapper.QueryableExtensions;
using Items.Application.Common.Interfaces;
using Items.Application.Uoms.Dtos;
using Items.Domain.Entities;
using Items.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Items.Application.Uoms;

public class UomService : IUomService
{
    private readonly IItemsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UomService> _logger;

    public UomService(IItemsDbContext context, IMapper mapper, ILogger<UomService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<UomDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UnitOfMeasures
            .AsNoTracking()
            .Where(u => u.IsActive)
            .OrderBy(u => u.Code)
            .ProjectTo<UomDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<UomDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var uom = await _context.UnitOfMeasures
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive, cancellationToken);

        if (uom is null)
            throw new NotFoundException(nameof(UnitOfMeasure), id);

        return _mapper.Map<UomDto>(uom);
    }

    public async Task<UomDto> CreateAsync(CreateUomRequest request, CancellationToken cancellationToken = default)
    {
        var codeExists = await _context.UnitOfMeasures
            .AnyAsync(u => u.Code == request.Code && u.IsActive, cancellationToken);

        if (codeExists)
            throw new ConflictException($"A UOM with code '{request.Code}' already exists.");

        var uom = _mapper.Map<UnitOfMeasure>(request);

        await _context.UnitOfMeasures.AddAsync(uom, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created UOM {Code} (Id={Id})", uom.Code, uom.Id);
        return _mapper.Map<UomDto>(uom);
    }

    public async Task<UomDto> UpdateAsync(int id, UpdateUomRequest request, CancellationToken cancellationToken = default)
    {
        var uom = await _context.UnitOfMeasures
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive, cancellationToken);

        if (uom is null)
            throw new NotFoundException(nameof(UnitOfMeasure), id);

        var codeExists = await _context.UnitOfMeasures
            .AnyAsync(u => u.Code == request.Code && u.Id != id && u.IsActive, cancellationToken);

        if (codeExists)
            throw new ConflictException($"A UOM with code '{request.Code}' already exists.");

        _mapper.Map(request, uom);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated UOM {Code} (Id={Id})", uom.Code, uom.Id);
        return _mapper.Map<UomDto>(uom);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var uom = await _context.UnitOfMeasures
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive, cancellationToken);

        if (uom is null)
            throw new NotFoundException(nameof(UnitOfMeasure), id);

        uom.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Soft-deleted UOM {Code} (Id={Id})", uom.Code, uom.Id);
    }
}
