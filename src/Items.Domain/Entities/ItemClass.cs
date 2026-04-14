using Items.Domain.Common;
using Items.Domain.Enums;

namespace Items.Domain.Entities;

public class ItemClass : BaseEntity
{
    // Section 1: Basic Identification
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Section 2: Nature & Flow
    public ItemNature ItemNature { get; set; }
    public ItemType? ItemType { get; set; }
    public bool Purchasable { get; set; }
    public bool Saleable { get; set; }

    // Section 3: Inventory & Valuation
    public bool InventoryManaged { get; set; }
    public ValuationMethod? ValuationMethod { get; set; }
    public decimal AllowExcessGrnPercent { get; set; }
    public IssueMethod IssueMethod { get; set; }
    public bool WoReceiptCostingViaExtraIssue { get; set; }

    // Section 4: Traceability
    public TraceabilityLevel TraceabilityLevel { get; set; }
    public bool InwardNumberTracking { get; set; }
    public bool HeatNumberTracking { get; set; }
    public bool ShelfLifeApplicable { get; set; }
    public bool? AllowIssueIfExpired { get; set; }
    public int? IgnoreDayInExpiry { get; set; }
    public int? CannotSellExpiredInNextDays { get; set; }

    // Section 5: Quality Control
    public bool QcRequired { get; set; }
    public bool TpiRequired { get; set; }
    public TpiTriggerStage? TpiTriggerStage { get; set; }
    public bool TpiCertificateMandatory { get; set; }
    public bool TpiCaptureRemarks { get; set; }
    public bool BlockDispatchUntilTpiAccepted { get; set; }
    public bool AllowTpiOverrideAtSo { get; set; }

    // Section 6: Planning & MRP
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

    // Section 7: Dimensions
    public bool DimensionBasedItem { get; set; }
    public bool BomQtyInWeightLengthWithDimensions { get; set; }
    public bool NeedDimensionWiseStockKeeping { get; set; }
    public bool NeedDimensionWiseConsumptionInBom { get; set; }
    public bool LongItem { get; set; }
    public bool BomQtyInDimensionsAndPieces { get; set; }

    // Section 8: Finance
    public string SalesAccountGroup { get; set; } = string.Empty;
    public string SalesAccount { get; set; } = string.Empty;
    public string PurchaseAccountGroup { get; set; } = string.Empty;
    public string PurchaseAccount { get; set; } = string.Empty;
}
