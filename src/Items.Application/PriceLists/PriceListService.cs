using AutoMapper;
using AutoMapper.QueryableExtensions;
using Items.Application.Common;
using Items.Application.Common.Interfaces;
using Items.Application.PriceLists.Dtos;
using Items.Domain.Entities;
using Items.Domain.Enums;
using Items.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Items.Application.PriceLists;

public class PriceListService : IPriceListService
{
    private readonly IItemsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<PriceListService> _logger;

    public PriceListService(IItemsDbContext context, IMapper mapper, ILogger<PriceListService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<PriceListSummaryDto>> GetAllAsync(
        int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.PriceLists
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.Priority)
            .ThenBy(p => p.Name);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<PriceListSummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<PriceListSummaryDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public async Task<PriceListDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var priceList = await _context.PriceLists
            .AsNoTracking()
            .Include(p => p.LineItems).ThenInclude(li => li.Item)
            .Include(p => p.LineItems).ThenInclude(li => li.ItemClass)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);

        if (priceList is null)
            throw new NotFoundException(nameof(PriceList), id);

        return _mapper.Map<PriceListDto>(priceList);
    }

    public async Task<PriceListDto> CreateAsync(
        CreatePriceListRequest request,
        CancellationToken cancellationToken = default)
    {
        var nameTaken = await _context.PriceLists
            .AnyAsync(p => p.Name == request.Name && p.IsActive, cancellationToken);
        if (nameTaken)
            throw new ConflictException($"A price list named '{request.Name}' already exists.");

        var code = await GenerateCodeAsync(cancellationToken);

        var priceList = _mapper.Map<PriceList>(request);
        priceList.Code = code;

        // LineItems mapped separately — FinalUnitPrice computed per item
        priceList.LineItems = request.LineItems.Select(li =>
        {
            var e = _mapper.Map<PriceListItem>(li);
            e.FinalUnitPrice = ComputeFinalPrice(li.BasePrice, li.DiscountType, li.DiscountValue);
            return e;
        }).ToList();

        _context.PriceLists.Add(priceList);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created PriceList {Code} (Id={Id})", priceList.Code, priceList.Id);

        return await GetByIdAsync(priceList.Id, cancellationToken);
    }

    public async Task<PriceListDto> UpdateAsync(
        int id,
        UpdatePriceListRequest request,
        CancellationToken cancellationToken = default)
    {
        var priceList = await _context.PriceLists
            .Include(p => p.LineItems)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);

        if (priceList is null)
            throw new NotFoundException(nameof(PriceList), id);

        var nameTaken = await _context.PriceLists
            .AnyAsync(p => p.Name == request.Name && p.Id != id && p.IsActive, cancellationToken);
        if (nameTaken)
            throw new ConflictException($"A price list named '{request.Name}' already exists.");

        // ApplicableTo and LineItems ignored per profile (non-editable / handled separately)
        _mapper.Map(request, priceList);

        priceList.LineItems.Clear();
        foreach (var li in request.LineItems)
        {
            var e = _mapper.Map<PriceListItem>(li);
            e.FinalUnitPrice = ComputeFinalPrice(li.BasePrice, li.DiscountType, li.DiscountValue);
            priceList.LineItems.Add(e);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated PriceList {Id}", id);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var priceList = await _context.PriceLists
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);

        if (priceList is null)
            throw new NotFoundException(nameof(PriceList), id);

        // Note: block if linked to active SO/contracts when Sales module is available.
        priceList.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Soft-deleted PriceList {Id}", id);
    }

    public async Task<PriceListDto> CloneAsync(
        int id,
        ClonePriceListRequest request,
        CancellationToken cancellationToken = default)
    {
        var source = await _context.PriceLists
            .AsNoTracking()
            .Include(p => p.LineItems)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);

        if (source is null)
            throw new NotFoundException(nameof(PriceList), id);

        var nameTaken = await _context.PriceLists
            .AnyAsync(p => p.Name == request.Name && p.IsActive, cancellationToken);
        if (nameTaken)
            throw new ConflictException($"A price list named '{request.Name}' already exists.");

        var code = await GenerateCodeAsync(cancellationToken);

        var clone = new PriceList
        {
            Code = code,
            ApplicableTo = source.ApplicableTo,
            Name = request.Name,
            Description = source.Description,
            Status = PriceListStatus.Draft,   // clones start as Draft per SRS clone logic
            Priority = source.Priority,
            Currency = source.Currency,
            TargetType = source.TargetType,
            TargetIds = source.TargetIds,
            PricingScopeType = source.PricingScopeType,
            IsActive = true,
            LineItems = source.LineItems.Select(li => new PriceListItem
            {
                ItemId = li.ItemId,
                ItemClassId = li.ItemClassId,
                MinQty = li.MinQty,
                MaxQty = li.MaxQty,
                BasePrice = li.BasePrice,
                DiscountType = li.DiscountType,
                DiscountValue = li.DiscountValue,
                FinalUnitPrice = li.FinalUnitPrice,
                RoundOffTo = li.RoundOffTo,
                ValidFrom = li.ValidFrom,
                ValidTo = li.ValidTo,
                IsActive = true
            }).ToList()
        };

        _context.PriceLists.Add(clone);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cloned PriceList {SourceId} → {NewId} ({Code})", id, clone.Id, clone.Code);

        return await GetByIdAsync(clone.Id, cancellationToken);
    }

    private static decimal ComputeFinalPrice(decimal basePrice, DiscountType? discountType, decimal? discountValue)
    {
        if (discountType is null || discountValue is null)
            return basePrice;

        return discountType == DiscountType.Percent
            ? basePrice * (1 - discountValue.Value / 100m)
            : basePrice - discountValue.Value;
    }

    private async Task<string> GenerateCodeAsync(CancellationToken cancellationToken)
    {
        var count = await _context.PriceLists.CountAsync(cancellationToken);
        return $"PL-{(count + 1):D6}";
    }
}
