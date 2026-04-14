using AutoMapper;
using AutoMapper.QueryableExtensions;
using Items.Application.Common;
using Items.Application.Common.Interfaces;
using Items.Application.ItemVendorMappings.Dtos;
using Items.Domain.Entities;
using Items.Domain.Enums;
using Items.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Items.Application.ItemVendorMappings;

public class ItemVendorMappingService : IItemVendorMappingService
{
    private readonly IItemsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ItemVendorMappingService> _logger;

    public ItemVendorMappingService(IItemsDbContext context, IMapper mapper, ILogger<ItemVendorMappingService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<ItemVendorMappingSummaryDto>> GetAllAsync(
        int page, int pageSize, int? itemId, int? vendorId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ItemVendorMappings
            .AsNoTracking()
            .Where(m => m.IsActive);

        if (itemId.HasValue)
            query = query.Where(m => m.ItemId == itemId.Value);

        if (vendorId.HasValue)
            query = query.Where(m => m.VendorId == vendorId.Value);

        var ordered = query.OrderBy(m => m.Code);
        var total = await ordered.CountAsync(cancellationToken);

        var items = await ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<ItemVendorMappingSummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<ItemVendorMappingSummaryDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public async Task<ItemVendorMappingDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var mapping = await _context.ItemVendorMappings
            .AsNoTracking()
            .Include(m => m.Item)
            .Include(m => m.PurchaseUoms).ThenInclude(u => u.PurchaseUom)
            .Include(m => m.Pricings)
            .FirstOrDefaultAsync(m => m.Id == id && m.IsActive, cancellationToken);

        if (mapping is null)
            throw new NotFoundException(nameof(ItemVendorMapping), id);

        return _mapper.Map<ItemVendorMappingDto>(mapping);
    }

    public async Task<ItemVendorMappingDto> CreateAsync(
        CreateItemVendorMappingRequest request,
        CancellationToken cancellationToken = default)
    {
        var itemExists = await _context.Items
            .AnyAsync(i => i.Id == request.ItemId && i.IsActive, cancellationToken);
        if (!itemExists)
            throw new NotFoundException(nameof(Item), request.ItemId);

        var duplicate = await _context.ItemVendorMappings
            .AnyAsync(m => m.ItemId == request.ItemId && m.VendorId == request.VendorId && m.IsActive, cancellationToken);
        if (duplicate)
            throw new ConflictException($"A mapping for item {request.ItemId} and vendor {request.VendorId} already exists.");

        if (request.IsPreferredVendor)
            await ClearPreferredVendorAsync(request.ItemId, null, cancellationToken);

        var code = await GenerateCodeAsync(cancellationToken);

        var mapping = _mapper.Map<ItemVendorMapping>(request);
        mapping.Code = code;

        // PurchaseUoms and Pricings mapped separately — ConversionRate and EffectivePurchasePrice require special logic
        mapping.PurchaseUoms = request.PurchaseUoms.Select(u =>
        {
            var e = _mapper.Map<ItemVendorPurchaseUom>(u);
            if (e.IsPerfectConversion) e.ConversionRate = 1m;
            return e;
        }).ToList();

        mapping.Pricings = request.Pricings.Select(p =>
        {
            var e = _mapper.Map<ItemVendorPricing>(p);
            e.EffectivePurchasePrice = ComputeEffectivePrice(p.BasePurchasePrice, p.DiscountType, p.DiscountValue);
            return e;
        }).ToList();

        _context.ItemVendorMappings.Add(mapping);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created ItemVendorMapping {Code} (Id={Id})", mapping.Code, mapping.Id);

        return await GetByIdAsync(mapping.Id, cancellationToken);
    }

    public async Task<ItemVendorMappingDto> UpdateAsync(
        int id,
        UpdateItemVendorMappingRequest request,
        CancellationToken cancellationToken = default)
    {
        var mapping = await _context.ItemVendorMappings
            .Include(m => m.PurchaseUoms)
            .Include(m => m.Pricings)
            .FirstOrDefaultAsync(m => m.Id == id && m.IsActive, cancellationToken);

        if (mapping is null)
            throw new NotFoundException(nameof(ItemVendorMapping), id);

        if (request.IsPreferredVendor && !mapping.IsPreferredVendor)
            await ClearPreferredVendorAsync(mapping.ItemId, id, cancellationToken);

        // ItemId, VendorId, PurchaseUoms, Pricings ignored per profile (non-editable / computed)
        _mapper.Map(request, mapping);

        mapping.PurchaseUoms.Clear();
        foreach (var u in request.PurchaseUoms)
        {
            var e = _mapper.Map<ItemVendorPurchaseUom>(u);
            if (e.IsPerfectConversion) e.ConversionRate = 1m;
            mapping.PurchaseUoms.Add(e);
        }

        mapping.Pricings.Clear();
        foreach (var p in request.Pricings)
        {
            var e = _mapper.Map<ItemVendorPricing>(p);
            e.EffectivePurchasePrice = ComputeEffectivePrice(p.BasePurchasePrice, p.DiscountType, p.DiscountValue);
            mapping.Pricings.Add(e);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated ItemVendorMapping {Id}", id);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var mapping = await _context.ItemVendorMappings
            .FirstOrDefaultAsync(m => m.Id == id && m.IsActive, cancellationToken);

        if (mapping is null)
            throw new NotFoundException(nameof(ItemVendorMapping), id);

        // Note: block if referenced in active POs when Purchase module is available.
        mapping.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Soft-deleted ItemVendorMapping {Id}", id);
    }

    public async Task<ItemVendorMappingDto> CloneAsync(
        int id,
        CloneItemVendorMappingRequest request,
        CancellationToken cancellationToken = default)
    {
        var source = await _context.ItemVendorMappings
            .AsNoTracking()
            .Include(m => m.PurchaseUoms)
            .Include(m => m.Pricings)
            .FirstOrDefaultAsync(m => m.Id == id && m.IsActive, cancellationToken);

        if (source is null)
            throw new NotFoundException(nameof(ItemVendorMapping), id);

        var targetItemId = request.OverrideItemId ?? source.ItemId;
        var targetVendorId = request.OverrideVendorId ?? source.VendorId;

        if (request.OverrideItemId.HasValue)
        {
            var itemExists = await _context.Items
                .AnyAsync(i => i.Id == targetItemId && i.IsActive, cancellationToken);
            if (!itemExists)
                throw new NotFoundException(nameof(Item), targetItemId);
        }

        var duplicate = await _context.ItemVendorMappings
            .AnyAsync(m => m.ItemId == targetItemId && m.VendorId == targetVendorId && m.IsActive, cancellationToken);
        if (duplicate)
            throw new ConflictException($"A mapping for item {targetItemId} and vendor {targetVendorId} already exists.");

        var code = await GenerateCodeAsync(cancellationToken);

        var clone = new ItemVendorMapping
        {
            Code = code,
            ItemId = targetItemId,
            VendorId = targetVendorId,
            VendorItemCode = source.VendorItemCode,
            VendorSku = source.VendorSku,
            IsPreferredVendor = false,  // clones are never preferred by default
            Moq = source.Moq,
            MaxOrderQty = source.MaxOrderQty,
            Eoq = source.Eoq,
            StandardLeadTimeDays = source.StandardLeadTimeDays,
            WarrantyDuration = source.WarrantyDuration,
            WarrantyDurationUnit = source.WarrantyDurationUnit,
            WarrantyStartBasis = source.WarrantyStartBasis,
            IsActive = true,
            PurchaseUoms = source.PurchaseUoms.Select(u => new ItemVendorPurchaseUom
            {
                PurchaseUomId = u.PurchaseUomId,
                IsPerfectConversion = u.IsPerfectConversion,
                ConversionRate = u.ConversionRate,
                TolerancePercent = u.TolerancePercent,
                IsActive = true
            }).ToList(),
            Pricings = source.Pricings.Select(p => new ItemVendorPricing
            {
                BasePurchasePrice = p.BasePurchasePrice,
                Currency = p.Currency,
                DiscountType = p.DiscountType,
                DiscountValue = p.DiscountValue,
                EffectivePurchasePrice = p.EffectivePurchasePrice,
                PriceValidFrom = p.PriceValidFrom,
                PriceValidTo = p.PriceValidTo,
                PaymentTerms = p.PaymentTerms,
                IsActive = true
            }).ToList()
        };

        _context.ItemVendorMappings.Add(clone);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cloned ItemVendorMapping {SourceId} → {NewId} ({Code})", id, clone.Id, clone.Code);

        return await GetByIdAsync(clone.Id, cancellationToken);
    }

    private async Task ClearPreferredVendorAsync(int itemId, int? excludeId, CancellationToken cancellationToken)
    {
        var existing = await _context.ItemVendorMappings
            .Where(m => m.ItemId == itemId && m.IsPreferredVendor && m.IsActive
                        && (excludeId == null || m.Id != excludeId))
            .ToListAsync(cancellationToken);

        foreach (var m in existing)
            m.IsPreferredVendor = false;
    }

    private static decimal ComputeEffectivePrice(decimal basePrice, DiscountType? discountType, decimal? discountValue)
    {
        if (discountType is null || discountValue is null)
            return basePrice;

        return discountType == DiscountType.Percent
            ? basePrice * (1 - discountValue.Value / 100m)
            : basePrice - discountValue.Value;
    }

    private async Task<string> GenerateCodeAsync(CancellationToken cancellationToken)
    {
        var count = await _context.ItemVendorMappings.CountAsync(cancellationToken);
        return $"IVM-{(count + 1):D6}";
    }
}
