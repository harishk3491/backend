using AutoMapper;
using AutoMapper.QueryableExtensions;
using Items.Application.Common;
using Items.Application.Common.Interfaces;
using Items.Application.ItemClasses.Dtos;
using Items.Domain.Entities;
using Items.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Items.Application.ItemClasses;

public class ItemClassService : IItemClassService
{
    private readonly IItemsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ItemClassService> _logger;

    public ItemClassService(IItemsDbContext context, IMapper mapper, ILogger<ItemClassService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<ItemClassSummaryDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.ItemClasses
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Code);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<ItemClassSummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<ItemClassSummaryDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public async Task<ItemClassDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var itemClass = await _context.ItemClasses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, cancellationToken);

        if (itemClass is null)
            throw new NotFoundException(nameof(ItemClass), id);

        return _mapper.Map<ItemClassDto>(itemClass);
    }

    public async Task<ItemClassDto> CreateAsync(CreateItemClassRequest request, CancellationToken cancellationToken = default)
    {
        var codeExists = await _context.ItemClasses
            .AnyAsync(c => c.Code == request.Code && c.IsActive, cancellationToken);

        if (codeExists)
            throw new ConflictException($"An Item Class with code '{request.Code}' already exists.");

        var itemClass = _mapper.Map<ItemClass>(request);

        await _context.ItemClasses.AddAsync(itemClass, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created ItemClass {Code} (Id={Id})", itemClass.Code, itemClass.Id);
        return _mapper.Map<ItemClassDto>(itemClass);
    }

    public async Task<ItemClassDto> UpdateAsync(int id, UpdateItemClassRequest request, CancellationToken cancellationToken = default)
    {
        var itemClass = await _context.ItemClasses
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, cancellationToken);

        if (itemClass is null)
            throw new NotFoundException(nameof(ItemClass), id);

        _mapper.Map(request, itemClass); // ItemNature ignored per profile (non-editable per SRS)
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated ItemClass {Code} (Id={Id})", itemClass.Code, itemClass.Id);
        return _mapper.Map<ItemClassDto>(itemClass);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var itemClass = await _context.ItemClasses
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, cancellationToken);

        if (itemClass is null)
            throw new NotFoundException(nameof(ItemClass), id);

        itemClass.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Soft-deleted ItemClass {Code} (Id={Id})", itemClass.Code, itemClass.Id);
    }

    public async Task<ItemClassDto> CloneAsync(int id, string newCode, string newName, CancellationToken cancellationToken = default)
    {
        var source = await _context.ItemClasses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, cancellationToken);

        if (source is null)
            throw new NotFoundException(nameof(ItemClass), id);

        var codeExists = await _context.ItemClasses
            .AnyAsync(c => c.Code == newCode && c.IsActive, cancellationToken);

        if (codeExists)
            throw new ConflictException($"An Item Class with code '{newCode}' already exists.");

        var clone = new ItemClass
        {
            Code = newCode,
            Name = newName,
            Description = source.Description,
            ItemNature = source.ItemNature,
            ItemType = source.ItemType,
            Purchasable = source.Purchasable,
            Saleable = source.Saleable,
            InventoryManaged = source.InventoryManaged,
            ValuationMethod = source.ValuationMethod,
            AllowExcessGrnPercent = source.AllowExcessGrnPercent,
            IssueMethod = source.IssueMethod,
            WoReceiptCostingViaExtraIssue = source.WoReceiptCostingViaExtraIssue,
            TraceabilityLevel = source.TraceabilityLevel,
            InwardNumberTracking = source.InwardNumberTracking,
            HeatNumberTracking = source.HeatNumberTracking,
            ShelfLifeApplicable = source.ShelfLifeApplicable,
            AllowIssueIfExpired = source.AllowIssueIfExpired,
            IgnoreDayInExpiry = source.IgnoreDayInExpiry,
            CannotSellExpiredInNextDays = source.CannotSellExpiredInNextDays,
            QcRequired = source.QcRequired,
            TpiRequired = source.TpiRequired,
            TpiTriggerStage = source.TpiTriggerStage,
            TpiCertificateMandatory = source.TpiCertificateMandatory,
            TpiCaptureRemarks = source.TpiCaptureRemarks,
            BlockDispatchUntilTpiAccepted = source.BlockDispatchUntilTpiAccepted,
            AllowTpiOverrideAtSo = source.AllowTpiOverrideAtSo,
            IndentMandatory = source.IndentMandatory,
            FastMovingThreshold = source.FastMovingThreshold,
            SlowMovingThreshold = source.SlowMovingThreshold,
            OrderReleaseLeadTime = source.OrderReleaseLeadTime,
            DeliveryReleaseLeadTime = source.DeliveryReleaseLeadTime,
            MinimumProductionLogic = source.MinimumProductionLogic,
            MinimumProductionQty = source.MinimumProductionQty,
            ProductionBatchMultiple = source.ProductionBatchMultiple,
            AutoAllocation = source.AutoAllocation,
            AutoAllocationOn = source.AutoAllocationOn,
            DimensionBasedItem = source.DimensionBasedItem,
            BomQtyInWeightLengthWithDimensions = source.BomQtyInWeightLengthWithDimensions,
            NeedDimensionWiseStockKeeping = source.NeedDimensionWiseStockKeeping,
            NeedDimensionWiseConsumptionInBom = source.NeedDimensionWiseConsumptionInBom,
            LongItem = source.LongItem,
            BomQtyInDimensionsAndPieces = source.BomQtyInDimensionsAndPieces,
            SalesAccountGroup = source.SalesAccountGroup,
            SalesAccount = source.SalesAccount,
            PurchaseAccountGroup = source.PurchaseAccountGroup,
            PurchaseAccount = source.PurchaseAccount,
            IsActive = true
        };

        await _context.ItemClasses.AddAsync(clone, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cloned ItemClass {SourceCode} → {NewCode} (Id={Id})", source.Code, clone.Code, clone.Id);
        return _mapper.Map<ItemClassDto>(clone);
    }
}
