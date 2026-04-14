using Items.Domain.Entities;
using Items.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Items.Infrastructure.Persistence;

public interface IDbInitializer
{
    Task SeedAsync();
}

public class DbInitializer : IDbInitializer
{
    private readonly ItemsDbContext _ctx;

    public DbInitializer(ItemsDbContext ctx) => _ctx = ctx;

    public async Task SeedAsync()
    {
        // ── 1. UnitOfMeasure ────────────────────────────────────────────────
        if (!await _ctx.UnitOfMeasures.AnyAsync())
        {
            _ctx.UnitOfMeasures.AddRange(
                new UnitOfMeasure { Code = "PCS", Name = "Pieces",        Description = "Individual discrete unit count", IsActive = true },
                new UnitOfMeasure { Code = "KG",  Name = "Kilograms",     Description = "Weight measurement",             IsActive = true },
                new UnitOfMeasure { Code = "MT",  Name = "Metric Tonne",  Description = "1 MT = 1 000 KG; bulk steel purchase UOM", IsActive = true },
                new UnitOfMeasure { Code = "BOX", Name = "Box",           Description = "Packaging box (10 PCS per box)", IsActive = true }
            );
            await _ctx.SaveChangesAsync();
        }

        // Resolve IDs by code for FK use below
        var uoms = await _ctx.UnitOfMeasures.AsNoTracking()
            .ToDictionaryAsync(u => u.Code, u => u.Id);

        // ── 2. ItemSettings (singleton) ─────────────────────────────────────
        if (!await _ctx.ItemSettings.AnyAsync())
        {
            _ctx.ItemSettings.Add(new ItemSettings
            {
                ItemCodeMaxLength              = 20,
                AllowItemLevelCustomization    = true,
                LogItemClassOverrides          = true,
                AllowDuplicateNames            = false,
                AllowTagNumberInSalesDocs      = true,
                EnableAutoBatchSerialGeneration = true,
                AutoGenPrefix                  = "BTH",
                AutoGenSeparator               = "-",
                AutoGenSequenceStart           = 1000,
                AutoGenSequenceIncrement       = 1,
                CopyDescription                = true,
                CopyTechnicalSpecs             = true,
                CopyPricingDetails             = true,
                CopyTaxDetails                 = true,
                CopyItemClassDefaults          = true,
                CopyAttachments                = false,
                EcnEntryPermission             = true,
                CcnNumberingBasis              = CcnNumberingBasis.ParentItem,
                IsActive                       = true
            });
            await _ctx.SaveChangesAsync();
        }

        // ── 3. ItemClass ────────────────────────────────────────────────────
        if (!await _ctx.ItemClasses.AnyAsync())
        {
            _ctx.ItemClasses.AddRange(
                new ItemClass
                {
                    Code                    = "IC-000001",
                    Name                    = "Steel Raw Materials",
                    Description             = "Raw material class for steel and ferrous metals",
                    ItemNature              = ItemNature.Goods,
                    ItemType                = ItemType.RM,
                    Purchasable             = true,
                    Saleable                = false,
                    InventoryManaged        = true,
                    ValuationMethod         = ValuationMethod.Fifo,
                    AllowExcessGrnPercent   = 5,
                    IssueMethod             = IssueMethod.Manual,
                    WoReceiptCostingViaExtraIssue = false,
                    TraceabilityLevel       = TraceabilityLevel.Batch,
                    InwardNumberTracking    = true,
                    HeatNumberTracking      = true,
                    ShelfLifeApplicable     = false,
                    QcRequired              = true,
                    TpiRequired             = false,
                    IndentMandatory         = true,
                    FastMovingThreshold     = 200,
                    SlowMovingThreshold     = 50,
                    OrderReleaseLeadTime    = 3,
                    DeliveryReleaseLeadTime = 2,
                    SalesAccountGroup       = "Revenue",
                    SalesAccount            = "4100",
                    PurchaseAccountGroup    = "COGS",
                    PurchaseAccount         = "5100",
                    IsActive                = true
                },
                new ItemClass
                {
                    Code                    = "IC-000002",
                    Name                    = "Electronic Assemblies",
                    Description             = "Finished electronic assembly sub-modules",
                    ItemNature              = ItemNature.Goods,
                    ItemType                = ItemType.AS,
                    Purchasable             = false,
                    Saleable                = true,
                    InventoryManaged        = true,
                    ValuationMethod         = ValuationMethod.MovingAverage,
                    AllowExcessGrnPercent   = 0,
                    IssueMethod             = IssueMethod.Manual,
                    WoReceiptCostingViaExtraIssue = false,
                    TraceabilityLevel       = TraceabilityLevel.Serial,
                    InwardNumberTracking    = false,
                    HeatNumberTracking      = false,
                    ShelfLifeApplicable     = false,
                    QcRequired              = true,
                    TpiRequired             = true,
                    TpiTriggerStage         = TpiTriggerStage.PreDispatch,
                    TpiCertificateMandatory = true,
                    TpiCaptureRemarks       = true,
                    BlockDispatchUntilTpiAccepted = true,
                    AllowTpiOverrideAtSo    = false,
                    IndentMandatory         = false,
                    FastMovingThreshold     = 500,
                    SlowMovingThreshold     = 100,
                    OrderReleaseLeadTime    = 0,
                    DeliveryReleaseLeadTime = 0,
                    SalesAccountGroup       = "Revenue",
                    SalesAccount            = "4200",
                    PurchaseAccountGroup    = "COGS",
                    PurchaseAccount         = "5200",
                    IsActive                = true
                }
            );
            await _ctx.SaveChangesAsync();
        }

        var classes = await _ctx.ItemClasses.AsNoTracking()
            .ToDictionaryAsync(c => c.Code, c => c.Id);

        // ── 4. Item ─────────────────────────────────────────────────────────
        if (!await _ctx.Items.AnyAsync())
        {
            _ctx.Items.AddRange(
                new Item
                {
                    Code                    = "ITM-000001",
                    Name                    = "Mild Steel Plate 10mm",
                    Description             = "Hot-rolled mild steel plate, 10 mm thickness, IS 2062 Grade A",
                    Sku                     = "MS-PLATE-10MM",
                    ItemClassId             = classes["IC-000001"],
                    ItemNature              = ItemNature.Goods,
                    ItemType                = ItemType.RM,
                    Purchasable             = true,
                    Saleable                = false,
                    InventoryManaged        = true,
                    ValuationMethod         = ValuationMethod.Fifo,
                    AllowExcessGrnPercent   = 5,
                    IssueMethod             = IssueMethod.Manual,
                    TraceabilityLevel       = TraceabilityLevel.Batch,
                    InwardNumberTracking    = true,
                    HeatNumberTracking      = true,
                    ShelfLifeApplicable     = false,
                    QcRequired              = true,
                    IndentMandatory         = true,
                    FastMovingThreshold     = 200,
                    SlowMovingThreshold     = 50,
                    OrderReleaseLeadTime    = 3,
                    DeliveryReleaseLeadTime = 2,
                    SalesAccountGroup       = "Revenue",
                    SalesAccount            = "4100",
                    PurchaseAccountGroup    = "COGS",
                    PurchaseAccount         = "5100",
                    BaseUomId               = uoms["KG"],
                    MaterialOfConstruction  = "Mild Steel IS 2062",
                    HsnSacCode              = "72085100",
                    TaxPreference           = TaxPreference.Taxable,
                    TaxRate                 = 18,
                    CountryOfOrigin         = "IN",
                    PurchaseCost            = 58.50m,
                    DiscountEligible        = false,
                    IsActive                = true
                },
                new Item
                {
                    Code                    = "ITM-000002",
                    Name                    = "Precision PCB Assembly Rev 2",
                    Description             = "High-precision printed circuit board assembly, revision 2",
                    Sku                     = "PCB-ASM-REV2",
                    ItemClassId             = classes["IC-000002"],
                    ItemNature              = ItemNature.Goods,
                    ItemType                = ItemType.AS,
                    Purchasable             = false,
                    Saleable                = true,
                    InventoryManaged        = true,
                    ValuationMethod         = ValuationMethod.MovingAverage,
                    AllowExcessGrnPercent   = 0,
                    IssueMethod             = IssueMethod.Manual,
                    TraceabilityLevel       = TraceabilityLevel.Serial,
                    InwardNumberTracking    = false,
                    HeatNumberTracking      = false,
                    ShelfLifeApplicable     = false,
                    QcRequired              = true,
                    TpiRequired             = true,
                    TpiTriggerStage         = TpiTriggerStage.PreDispatch,
                    TpiCertificateMandatory = true,
                    TpiCaptureRemarks       = true,
                    BlockDispatchUntilTpiAccepted = true,
                    IndentMandatory         = false,
                    FastMovingThreshold     = 500,
                    SlowMovingThreshold     = 100,
                    OrderReleaseLeadTime    = 0,
                    DeliveryReleaseLeadTime = 0,
                    SalesAccountGroup       = "Revenue",
                    SalesAccount            = "4200",
                    PurchaseAccountGroup    = "COGS",
                    PurchaseAccount         = "5200",
                    BaseUomId               = uoms["PCS"],
                    MaterialOfConstruction  = "FR4 Fibreglass",
                    HsnSacCode              = "85340000",
                    TaxPreference           = TaxPreference.Taxable,
                    TaxRate                 = 18,
                    CountryOfOrigin         = "IN",
                    SalesPrice              = 2450m,
                    MmfgItem                = true,
                    MinMassManufacturingQty = 50,
                    DiscountEligible        = true,
                    IsActive                = true
                }
            );
            await _ctx.SaveChangesAsync();
        }

        var items = await _ctx.Items.AsNoTracking()
            .ToDictionaryAsync(i => i.Code, i => i.Id);

        // ── 5. ItemAlternateUom ─────────────────────────────────────────────
        if (!await _ctx.ItemAlternateUoms.AnyAsync())
        {
            _ctx.ItemAlternateUoms.AddRange(
                new ItemAlternateUom
                {
                    ItemId           = items["ITM-000001"],
                    UomId            = uoms["MT"],
                    ConversionFactor = 1000,
                    IsPerfect        = false,
                    IsActive         = true
                },
                new ItemAlternateUom
                {
                    ItemId           = items["ITM-000002"],
                    UomId            = uoms["BOX"],
                    ConversionFactor = 10,
                    IsPerfect        = true,
                    IsActive         = true
                }
            );
            await _ctx.SaveChangesAsync();
        }

        // ── 6. ItemTechnicalSpec ────────────────────────────────────────────
        if (!await _ctx.ItemTechnicalSpecs.AnyAsync())
        {
            _ctx.ItemTechnicalSpecs.AddRange(
                new ItemTechnicalSpec
                {
                    ItemId          = items["ITM-000001"],
                    CapabilityType  = "Tensile Strength",
                    CapabilityValue = "410 MPa",
                    IsActive        = true
                },
                new ItemTechnicalSpec
                {
                    ItemId          = items["ITM-000002"],
                    CapabilityType  = "Operating Voltage",
                    CapabilityValue = "3.3 V – 5 V DC",
                    IsActive        = true
                }
            );
            await _ctx.SaveChangesAsync();
        }

        // ── 7. ItemDrawing ──────────────────────────────────────────────────
        if (!await _ctx.ItemDrawings.AnyAsync())
        {
            _ctx.ItemDrawings.AddRange(
                new ItemDrawing
                {
                    ItemId                  = items["ITM-000001"],
                    DrawingName             = "Plate Cutting Layout",
                    DrawingNumber           = "DRW-MS-10-001",
                    Revision                = "Rev A",
                    Size                    = "A3",
                    ShowInProductionBooking = false,
                    IsActive                = true
                },
                new ItemDrawing
                {
                    ItemId                  = items["ITM-000002"],
                    DrawingName             = "PCB Schematic Rev 2",
                    DrawingNumber           = "DRW-PCB-002-R2",
                    Revision                = "Rev B",
                    Size                    = "A4",
                    ShowInProductionBooking = true,
                    IsActive                = true
                }
            );
            await _ctx.SaveChangesAsync();
        }

        // ── 8. ItemWarehouseThreshold ───────────────────────────────────────
        if (!await _ctx.ItemWarehouseThresholds.AnyAsync())
        {
            _ctx.ItemWarehouseThresholds.AddRange(
                new ItemWarehouseThreshold
                {
                    ItemId        = items["ITM-000001"],
                    WarehouseName = "Main Store",
                    MinThreshold  = 500,
                    MaxThreshold  = 5000,
                    ReorderQty    = 1000,
                    OpeningQty    = 2000,
                    OpeningRate   = 57.00m,
                    OpeningDate   = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive      = true
                },
                new ItemWarehouseThreshold
                {
                    ItemId        = items["ITM-000002"],
                    WarehouseName = "FG Warehouse",
                    MinThreshold  = 20,
                    MaxThreshold  = 200,
                    ReorderQty    = 50,
                    OpeningQty    = 75,
                    OpeningRate   = 2200.00m,
                    OpeningDate   = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive      = true
                }
            );
            await _ctx.SaveChangesAsync();
        }

        // ── 9. ItemVendorMapping ────────────────────────────────────────────
        if (!await _ctx.ItemVendorMappings.AnyAsync())
        {
            _ctx.ItemVendorMappings.AddRange(
                new ItemVendorMapping
                {
                    Code                  = "IVM-000001",
                    ItemId                = items["ITM-000001"],
                    VendorId              = 101,
                    VendorItemCode        = "SS-GRD-A-2062",
                    IsPreferredVendor     = true,
                    Moq                   = 500,
                    StandardLeadTimeDays  = 7,
                    IsActive              = true
                },
                new ItemVendorMapping
                {
                    Code                  = "IVM-000002",
                    ItemId                = items["ITM-000002"],
                    VendorId              = 205,
                    VendorItemCode        = "PCB-V2-EXT",
                    IsPreferredVendor     = true,
                    Moq                   = 10,
                    StandardLeadTimeDays  = 14,
                    IsActive              = true
                }
            );
            await _ctx.SaveChangesAsync();
        }

        var vendorMappings = await _ctx.ItemVendorMappings.AsNoTracking()
            .ToDictionaryAsync(v => v.Code, v => v.Id);

        // ── 10. ItemVendorPurchaseUom ───────────────────────────────────────
        if (!await _ctx.ItemVendorPurchaseUoms.AnyAsync())
        {
            _ctx.ItemVendorPurchaseUoms.AddRange(
                new ItemVendorPurchaseUom
                {
                    ItemVendorMappingId   = vendorMappings["IVM-000001"],
                    PurchaseUomId         = uoms["MT"],
                    IsPerfectConversion   = false,
                    ConversionRate        = 1000,
                    TolerancePercent      = 3,
                    IsActive              = true
                },
                new ItemVendorPurchaseUom
                {
                    ItemVendorMappingId   = vendorMappings["IVM-000002"],
                    PurchaseUomId         = uoms["PCS"],
                    IsPerfectConversion   = true,
                    ConversionRate        = 1,
                    TolerancePercent      = 0,
                    IsActive              = true
                }
            );
            await _ctx.SaveChangesAsync();
        }

        // ── 11. ItemVendorPricing ───────────────────────────────────────────
        if (!await _ctx.ItemVendorPricings.AnyAsync())
        {
            _ctx.ItemVendorPricings.AddRange(
                new ItemVendorPricing
                {
                    ItemVendorMappingId    = vendorMappings["IVM-000001"],
                    BasePurchasePrice      = 58500m,
                    Currency               = "INR",
                    DiscountType           = DiscountType.Percent,
                    DiscountValue          = 2,
                    EffectivePurchasePrice = 57330m,
                    PriceValidFrom         = new DateOnly(2026, 1, 1),
                    PriceValidTo           = new DateOnly(2026, 12, 31),
                    PaymentTerms           = "Net 30",
                    IsActive               = true
                },
                new ItemVendorPricing
                {
                    ItemVendorMappingId    = vendorMappings["IVM-000002"],
                    BasePurchasePrice      = 2200m,
                    Currency               = "INR",
                    DiscountType           = DiscountType.FixedAmount,
                    DiscountValue          = 50,
                    EffectivePurchasePrice = 2150m,
                    PriceValidFrom         = new DateOnly(2026, 4, 1),
                    PriceValidTo           = new DateOnly(2026, 9, 30),
                    PaymentTerms           = "50% Advance",
                    IsActive               = true
                }
            );
            await _ctx.SaveChangesAsync();
        }

        // ── 12. PriceList ───────────────────────────────────────────────────
        if (!await _ctx.PriceLists.AnyAsync())
        {
            _ctx.PriceLists.AddRange(
                new PriceList
                {
                    Code             = "PL-000001",
                    ApplicableTo     = PriceListApplicableTo.Sales,
                    Name             = "Domestic Wholesale 2026",
                    Status           = PriceListStatus.Active,
                    Priority         = 100,
                    Currency         = "INR",
                    TargetType       = PriceListTargetType.AllCustomers,
                    PricingScopeType = PricingScopeType.SelectedItems,
                    IsActive         = true
                },
                new PriceList
                {
                    Code             = "PL-000002",
                    ApplicableTo     = PriceListApplicableTo.Purchase,
                    Name             = "Steel Vendor Contract H1 2026",
                    Status           = PriceListStatus.Active,
                    Priority         = 80,
                    Currency         = "INR",
                    TargetType       = PriceListTargetType.AllVendors,
                    PricingScopeType = PricingScopeType.SelectedItems,
                    IsActive         = true
                }
            );
            await _ctx.SaveChangesAsync();
        }

        var priceLists = await _ctx.PriceLists.AsNoTracking()
            .ToDictionaryAsync(p => p.Code, p => p.Id);

        // ── 13. PriceListItem ───────────────────────────────────────────────
        if (!await _ctx.PriceListItems.AnyAsync())
        {
            _ctx.PriceListItems.AddRange(
                new PriceListItem
                {
                    PriceListId    = priceLists["PL-000001"],
                    ItemId         = items["ITM-000002"],
                    MinQty         = 1,
                    MaxQty         = 49,
                    BasePrice      = 2450m,
                    DiscountType   = DiscountType.Percent,
                    DiscountValue  = 5,
                    FinalUnitPrice = 2327.50m,
                    ValidFrom      = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    ValidTo        = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
                    IsActive       = true
                },
                new PriceListItem
                {
                    PriceListId    = priceLists["PL-000001"],
                    ItemId         = items["ITM-000002"],
                    MinQty         = 50,
                    BasePrice      = 2450m,
                    DiscountType   = DiscountType.Percent,
                    DiscountValue  = 10,
                    FinalUnitPrice = 2205m,
                    ValidFrom      = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    ValidTo        = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
                    IsActive       = true
                }
            );
            await _ctx.SaveChangesAsync();
        }
    }
}
