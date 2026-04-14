using Items.Domain.Enums;

namespace Items.Application.Items.Dtos;

public class ItemDto
{
    public int Id { get; set; }

    // Section 1: Identity
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int? ParentItemId { get; set; }
    public string? VariantName { get; set; }
    public string? VariantCode { get; set; }

    // Section 2: Classification
    public int? ItemClassId { get; set; }
    public string? ItemClassName { get; set; }
    public ItemNature? ItemNature { get; set; }
    public ItemType? ItemType { get; set; }
    public string? ReferenceCode { get; set; }
    public string? Manufacturer { get; set; }
    public bool KanbanItem { get; set; }
    public bool MmfgItem { get; set; }
    public decimal? MinMassManufacturingQty { get; set; }
    public bool WantQrCode { get; set; }

    // Customizable attributes
    public bool Purchasable { get; set; }
    public bool Saleable { get; set; }
    public bool InventoryManaged { get; set; }
    public ValuationMethod? ValuationMethod { get; set; }
    public decimal AllowExcessGrnPercent { get; set; }
    public IssueMethod IssueMethod { get; set; }
    public bool WoReceiptCostingViaExtraIssue { get; set; }
    public TraceabilityLevel TraceabilityLevel { get; set; }
    public bool InwardNumberTracking { get; set; }
    public bool HeatNumberTracking { get; set; }
    public bool ShelfLifeApplicable { get; set; }
    public bool? AllowIssueIfExpired { get; set; }
    public int? IgnoreDayInExpiry { get; set; }
    public int? CannotSellExpiredInNextDays { get; set; }
    public bool QcRequired { get; set; }
    public bool TpiRequired { get; set; }
    public TpiTriggerStage? TpiTriggerStage { get; set; }
    public bool TpiCertificateMandatory { get; set; }
    public bool TpiCaptureRemarks { get; set; }
    public bool BlockDispatchUntilTpiAccepted { get; set; }
    public bool AllowTpiOverrideAtSo { get; set; }
    public bool IndentMandatory { get; set; }
    public decimal FastMovingThreshold { get; set; }
    public decimal SlowMovingThreshold { get; set; }
    public int OrderReleaseLeadTime { get; set; }
    public int DeliveryReleaseLeadTime { get; set; }
    public bool MinimumProductionLogic { get; set; }
    public decimal? MinimumProductionQty { get; set; }
    public decimal? ProductionBatchMultiple { get; set; }
    public bool AutoAllocation { get; set; }
    public AutoAllocationOn? AutoAllocationOn { get; set; }
    public bool DimensionBasedItem { get; set; }
    public bool NeedDimensionWiseStockKeeping { get; set; }
    public bool NeedDimensionWiseConsumptionInBom { get; set; }
    public bool LongItem { get; set; }
    public bool BomQtyInDimensionsAndPieces { get; set; }
    public string SalesAccountGroup { get; set; } = string.Empty;
    public string SalesAccount { get; set; } = string.Empty;
    public string PurchaseAccountGroup { get; set; } = string.Empty;
    public string PurchaseAccount { get; set; } = string.Empty;

    // Section 4: Units & Physical
    public int BaseUomId { get; set; }
    public string BaseUomCode { get; set; } = string.Empty;
    public string MaterialOfConstruction { get; set; } = string.Empty;
    public decimal? StandardWeight { get; set; }

    // Section 6: Tax & Regulatory
    public string HsnSacCode { get; set; } = string.Empty;
    public TaxPreference TaxPreference { get; set; }
    public decimal? TaxRate { get; set; }
    public string CountryOfOrigin { get; set; } = string.Empty;

    // Section 7: Commercial
    public decimal? PurchaseCost { get; set; }
    public decimal? SalesPrice { get; set; }
    public decimal? SubContractingCost { get; set; }
    public bool DiscountEligible { get; set; }

    // Section 8: Tool Details
    public bool? IsToolConsumable { get; set; }
    public ConsumptionType? ConsumptionType { get; set; }
    public int? ConsumptionNumberOfTimes { get; set; }
    public int? ConsumptionPeriodValue { get; set; }
    public ConsumptionPeriodUnit? ConsumptionPeriodUnit { get; set; }
    public ConsumptionStartBasis? ConsumptionStartBasis { get; set; }
    public bool? MaintenanceRequired { get; set; }
    public int? MaintenanceFrequencyValue { get; set; }
    public FrequencyUnit? MaintenanceFrequencyUnit { get; set; }
    public bool? CalibrationRequired { get; set; }
    public int? CalibrationFrequencyValue { get; set; }
    public FrequencyUnit? CalibrationFrequencyUnit { get; set; }
    public decimal? MaximumCapacity { get; set; }
    public PostExhaustionAction? PostExhaustionAction { get; set; }

    // Sub-entities
    public List<ItemAlternateUomDto> AlternateUoms { get; set; } = new();
    public List<ItemTechnicalSpecDto> TechnicalSpecs { get; set; } = new();
    public List<ItemDrawingDto> Drawings { get; set; } = new();
    public List<ItemWarehouseThresholdDto> WarehouseThresholds { get; set; } = new();

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
