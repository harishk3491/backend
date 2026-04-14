using AutoMapper;
using AutoMapper.QueryableExtensions;
using Items.Application.Common;
using Items.Application.Common.Interfaces;
using Items.Application.Items.Dtos;
using Items.Domain.Entities;
using Items.Domain.Enums;
using Items.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Items.Application.Items;

public class ItemService : IItemService
{
    private readonly IItemsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ItemService> _logger;

    public ItemService(IItemsDbContext context, IMapper mapper, ILogger<ItemService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<ItemSummaryDto>> GetAllAsync(
        int page, int pageSize,
        int? itemClassId, ItemType? itemType, ItemNature? itemNature,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Items
            .AsNoTracking()
            .Where(i => i.IsActive);

        if (itemClassId.HasValue)
            query = query.Where(i => i.ItemClassId == itemClassId.Value);

        if (itemType.HasValue)
            query = query.Where(i => i.ItemType == itemType.Value);

        if (itemNature.HasValue)
            query = query.Where(i => i.ItemNature == itemNature.Value);

        var ordered = query.OrderBy(i => i.Code);

        var total = await ordered.CountAsync(cancellationToken);

        var items = await ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<ItemSummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<ItemSummaryDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public async Task<ItemDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _context.Items
            .AsNoTracking()
            .Include(i => i.ItemClass)
            .Include(i => i.BaseUom)
            .Include(i => i.AlternateUoms).ThenInclude(a => a.Uom)
            .Include(i => i.TechnicalSpecs)
            .Include(i => i.Drawings)
            .Include(i => i.WarehouseThresholds)
            .FirstOrDefaultAsync(i => i.Id == id && i.IsActive, cancellationToken);

        if (item is null)
            throw new NotFoundException(nameof(Item), id);

        return _mapper.Map<ItemDto>(item);
    }

    public async Task<ItemDto> CreateAsync(CreateItemRequest request, CancellationToken cancellationToken = default)
    {
        var skuExists = await _context.Items
            .AnyAsync(i => i.Sku == request.Sku && i.IsActive, cancellationToken);

        if (skuExists)
            throw new ConflictException($"An item with SKU '{request.Sku}' already exists.");

        var uomExists = await _context.UnitOfMeasures
            .AnyAsync(u => u.Id == request.BaseUomId && u.IsActive, cancellationToken);

        if (!uomExists)
            throw new NotFoundException(nameof(UnitOfMeasure), request.BaseUomId);

        var code = await GenerateItemCodeAsync(cancellationToken);

        var item = _mapper.Map<Item>(request);
        item.Code = code;

        // Sub-collections mapped separately; service sets FK (EF resolves ItemId on save)
        item.AlternateUoms = _mapper.Map<List<ItemAlternateUom>>(request.AlternateUoms);
        item.TechnicalSpecs = _mapper.Map<List<ItemTechnicalSpec>>(request.TechnicalSpecs);
        item.Drawings = _mapper.Map<List<ItemDrawing>>(request.Drawings);
        item.WarehouseThresholds = _mapper.Map<List<ItemWarehouseThreshold>>(request.WarehouseThresholds);

        await _context.Items.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created Item {Code} SKU={Sku} (Id={Id})", item.Code, item.Sku, item.Id);

        return await GetByIdAsync(item.Id, cancellationToken);
    }

    public async Task<ItemDto> UpdateAsync(int id, UpdateItemRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _context.Items
            .Include(i => i.AlternateUoms)
            .Include(i => i.TechnicalSpecs)
            .Include(i => i.Drawings)
            .Include(i => i.WarehouseThresholds)
            .FirstOrDefaultAsync(i => i.Id == id && i.IsActive, cancellationToken);

        if (item is null)
            throw new NotFoundException(nameof(Item), id);

        var skuExists = await _context.Items
            .AnyAsync(i => i.Sku == request.Sku && i.Id != id && i.IsActive, cancellationToken);

        if (skuExists)
            throw new ConflictException($"An item with SKU '{request.Sku}' already exists.");

        // ItemNature, dimension flags (DimensionBasedItem, NeedDimensionWise*, LongItem) ignored per profile
        _mapper.Map(request, item);

        // Replace sub-entity collections
        _context.ItemAlternateUoms.RemoveRange(item.AlternateUoms);
        _context.ItemTechnicalSpecs.RemoveRange(item.TechnicalSpecs);
        _context.ItemDrawings.RemoveRange(item.Drawings);
        _context.ItemWarehouseThresholds.RemoveRange(item.WarehouseThresholds);

        item.AlternateUoms = request.AlternateUoms.Select(a =>
        {
            var e = _mapper.Map<ItemAlternateUom>(a);
            e.ItemId = item.Id;
            return e;
        }).ToList();

        item.TechnicalSpecs = request.TechnicalSpecs.Select(s =>
        {
            var e = _mapper.Map<ItemTechnicalSpec>(s);
            e.ItemId = item.Id;
            return e;
        }).ToList();

        item.Drawings = request.Drawings.Select(d =>
        {
            var e = _mapper.Map<ItemDrawing>(d);
            e.ItemId = item.Id;
            return e;
        }).ToList();

        item.WarehouseThresholds = request.WarehouseThresholds.Select(w =>
        {
            var e = _mapper.Map<ItemWarehouseThreshold>(w);
            e.ItemId = item.Id;
            return e;
        }).ToList();

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated Item {Code} (Id={Id})", item.Code, item.Id);

        return await GetByIdAsync(item.Id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == id && i.IsActive, cancellationToken);

        if (item is null)
            throw new NotFoundException(nameof(Item), id);

        item.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Soft-deleted Item {Code} (Id={Id})", item.Code, item.Id);
    }

    public async Task<ItemDto> CloneAsync(int id, CloneItemRequest request, CancellationToken cancellationToken = default)
    {
        var source = await _context.Items
            .AsNoTracking()
            .Include(i => i.AlternateUoms)
            .Include(i => i.TechnicalSpecs)
            .Include(i => i.Drawings)
            .Include(i => i.WarehouseThresholds)
            .FirstOrDefaultAsync(i => i.Id == id && i.IsActive, cancellationToken);

        if (source is null)
            throw new NotFoundException(nameof(Item), id);

        var skuExists = await _context.Items
            .AnyAsync(i => i.Sku == request.Sku && i.IsActive, cancellationToken);

        if (skuExists)
            throw new ConflictException($"An item with SKU '{request.Sku}' already exists.");

        var code = await GenerateItemCodeAsync(cancellationToken);

        var clone = new Item
        {
            Code = code,
            Name = request.Name,
            Description = source.Description,
            Sku = request.Sku,
            ParentItemId = source.ParentItemId,
            VariantName = source.VariantName,
            VariantCode = source.VariantCode,
            ItemClassId = source.ItemClassId,
            ItemNature = source.ItemNature,
            ItemType = source.ItemType,
            ReferenceCode = source.ReferenceCode,
            Manufacturer = source.Manufacturer,
            KanbanItem = source.KanbanItem,
            MmfgItem = source.MmfgItem,
            MinMassManufacturingQty = source.MinMassManufacturingQty,
            WantQrCode = source.WantQrCode,
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
            NeedDimensionWiseStockKeeping = source.NeedDimensionWiseStockKeeping,
            NeedDimensionWiseConsumptionInBom = source.NeedDimensionWiseConsumptionInBom,
            LongItem = source.LongItem,
            BomQtyInDimensionsAndPieces = source.BomQtyInDimensionsAndPieces,
            SalesAccountGroup = source.SalesAccountGroup,
            SalesAccount = source.SalesAccount,
            PurchaseAccountGroup = source.PurchaseAccountGroup,
            PurchaseAccount = source.PurchaseAccount,
            BaseUomId = source.BaseUomId,
            MaterialOfConstruction = source.MaterialOfConstruction,
            StandardWeight = source.StandardWeight,
            HsnSacCode = source.HsnSacCode,
            TaxPreference = source.TaxPreference,
            TaxRate = source.TaxRate,
            CountryOfOrigin = source.CountryOfOrigin,
            PurchaseCost = source.PurchaseCost,
            SalesPrice = source.SalesPrice,
            SubContractingCost = source.SubContractingCost,
            DiscountEligible = source.DiscountEligible,
            IsToolConsumable = source.IsToolConsumable,
            ConsumptionType = source.ConsumptionType,
            ConsumptionNumberOfTimes = source.ConsumptionNumberOfTimes,
            ConsumptionPeriodValue = source.ConsumptionPeriodValue,
            ConsumptionPeriodUnit = source.ConsumptionPeriodUnit,
            ConsumptionStartBasis = source.ConsumptionStartBasis,
            MaintenanceRequired = source.MaintenanceRequired,
            MaintenanceFrequencyValue = source.MaintenanceFrequencyValue,
            MaintenanceFrequencyUnit = source.MaintenanceFrequencyUnit,
            CalibrationRequired = source.CalibrationRequired,
            CalibrationFrequencyValue = source.CalibrationFrequencyValue,
            CalibrationFrequencyUnit = source.CalibrationFrequencyUnit,
            MaximumCapacity = source.MaximumCapacity,
            PostExhaustionAction = source.PostExhaustionAction,
            IsActive = true,
            AlternateUoms = source.AlternateUoms.Select(a => new ItemAlternateUom
            {
                UomId = a.UomId,
                ConversionFactor = a.ConversionFactor,
                IsPerfect = a.IsPerfect,
                IsActive = true
            }).ToList(),
            TechnicalSpecs = source.TechnicalSpecs.Select(s => new ItemTechnicalSpec
            {
                CapabilityType = s.CapabilityType,
                CapabilityValue = s.CapabilityValue,
                IsActive = true
            }).ToList(),
            Drawings = source.Drawings.Select(d => new ItemDrawing
            {
                DrawingName = d.DrawingName,
                DrawingNumber = d.DrawingNumber,
                Revision = d.Revision,
                Size = d.Size,
                ShowInProductionBooking = d.ShowInProductionBooking,
                IsActive = true
            }).ToList(),
            WarehouseThresholds = source.WarehouseThresholds.Select(w => new ItemWarehouseThreshold
            {
                WarehouseName = w.WarehouseName,
                MinThreshold = w.MinThreshold,
                MaxThreshold = w.MaxThreshold,
                ReorderQty = w.ReorderQty,
                OpeningQty = w.OpeningQty,
                OpeningRate = w.OpeningRate,
                OpeningDate = w.OpeningDate,
                IsActive = true
            }).ToList()
        };

        await _context.Items.AddAsync(clone, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cloned Item {SourceCode} → {NewCode} SKU={Sku} (Id={Id})", source.Code, clone.Code, clone.Sku, clone.Id);

        return await GetByIdAsync(clone.Id, cancellationToken);
    }

    private async Task<string> GenerateItemCodeAsync(CancellationToken cancellationToken)
    {
        var count = await _context.Items.CountAsync(cancellationToken);
        return $"ITM-{(count + 1):D6}";
    }
}
