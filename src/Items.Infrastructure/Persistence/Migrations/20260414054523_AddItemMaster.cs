using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Items.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddItemMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParentItemId = table.Column<int>(type: "int", nullable: true),
                    VariantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VariantCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ItemClassId = table.Column<int>(type: "int", nullable: true),
                    ItemNature = table.Column<int>(type: "int", nullable: true),
                    ItemType = table.Column<int>(type: "int", nullable: true),
                    ReferenceCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    KanbanItem = table.Column<bool>(type: "bit", nullable: false),
                    MmfgItem = table.Column<bool>(type: "bit", nullable: false),
                    MinMassManufacturingQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    WantQrCode = table.Column<bool>(type: "bit", nullable: false),
                    Purchasable = table.Column<bool>(type: "bit", nullable: false),
                    Saleable = table.Column<bool>(type: "bit", nullable: false),
                    InventoryManaged = table.Column<bool>(type: "bit", nullable: false),
                    ValuationMethod = table.Column<int>(type: "int", nullable: true),
                    AllowExcessGrnPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IssueMethod = table.Column<int>(type: "int", nullable: false),
                    WoReceiptCostingViaExtraIssue = table.Column<bool>(type: "bit", nullable: false),
                    TraceabilityLevel = table.Column<int>(type: "int", nullable: false),
                    InwardNumberTracking = table.Column<bool>(type: "bit", nullable: false),
                    HeatNumberTracking = table.Column<bool>(type: "bit", nullable: false),
                    ShelfLifeApplicable = table.Column<bool>(type: "bit", nullable: false),
                    AllowIssueIfExpired = table.Column<bool>(type: "bit", nullable: true),
                    IgnoreDayInExpiry = table.Column<int>(type: "int", nullable: true),
                    CannotSellExpiredInNextDays = table.Column<int>(type: "int", nullable: true),
                    QcRequired = table.Column<bool>(type: "bit", nullable: false),
                    TpiRequired = table.Column<bool>(type: "bit", nullable: false),
                    TpiTriggerStage = table.Column<int>(type: "int", nullable: true),
                    TpiCertificateMandatory = table.Column<bool>(type: "bit", nullable: false),
                    TpiCaptureRemarks = table.Column<bool>(type: "bit", nullable: false),
                    BlockDispatchUntilTpiAccepted = table.Column<bool>(type: "bit", nullable: false),
                    AllowTpiOverrideAtSo = table.Column<bool>(type: "bit", nullable: false),
                    IndentMandatory = table.Column<bool>(type: "bit", nullable: false),
                    FastMovingThreshold = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SlowMovingThreshold = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    OrderReleaseLeadTime = table.Column<int>(type: "int", nullable: false),
                    DeliveryReleaseLeadTime = table.Column<int>(type: "int", nullable: false),
                    MinimumProductionLogic = table.Column<bool>(type: "bit", nullable: false),
                    MinimumProductionQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ProductionBatchMultiple = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    AutoAllocation = table.Column<bool>(type: "bit", nullable: false),
                    AutoAllocationOn = table.Column<int>(type: "int", nullable: true),
                    DimensionBasedItem = table.Column<bool>(type: "bit", nullable: false),
                    NeedDimensionWiseStockKeeping = table.Column<bool>(type: "bit", nullable: false),
                    NeedDimensionWiseConsumptionInBom = table.Column<bool>(type: "bit", nullable: false),
                    LongItem = table.Column<bool>(type: "bit", nullable: false),
                    BomQtyInDimensionsAndPieces = table.Column<bool>(type: "bit", nullable: false),
                    SalesAccountGroup = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SalesAccount = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PurchaseAccountGroup = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PurchaseAccount = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BaseUomId = table.Column<int>(type: "int", nullable: false),
                    MaterialOfConstruction = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StandardWeight = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    HsnSacCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TaxPreference = table.Column<int>(type: "int", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    CountryOfOrigin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PurchaseCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    SalesPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    SubContractingCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    DiscountEligible = table.Column<bool>(type: "bit", nullable: false),
                    IsToolConsumable = table.Column<bool>(type: "bit", nullable: true),
                    ConsumptionType = table.Column<int>(type: "int", nullable: true),
                    ConsumptionNumberOfTimes = table.Column<int>(type: "int", nullable: true),
                    ConsumptionPeriodValue = table.Column<int>(type: "int", nullable: true),
                    ConsumptionPeriodUnit = table.Column<int>(type: "int", nullable: true),
                    ConsumptionStartBasis = table.Column<int>(type: "int", nullable: true),
                    MaintenanceRequired = table.Column<bool>(type: "bit", nullable: true),
                    MaintenanceFrequencyValue = table.Column<int>(type: "int", nullable: true),
                    MaintenanceFrequencyUnit = table.Column<int>(type: "int", nullable: true),
                    CalibrationRequired = table.Column<bool>(type: "bit", nullable: true),
                    CalibrationFrequencyValue = table.Column<int>(type: "int", nullable: true),
                    CalibrationFrequencyUnit = table.Column<int>(type: "int", nullable: true),
                    MaximumCapacity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    PostExhaustionAction = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_ItemClasses_ItemClassId",
                        column: x => x.ItemClassId,
                        principalTable: "ItemClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Items_Items_ParentItemId",
                        column: x => x.ParentItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_UnitOfMeasures_BaseUomId",
                        column: x => x.BaseUomId,
                        principalTable: "UnitOfMeasures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemAlternateUoms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    UomId = table.Column<int>(type: "int", nullable: false),
                    ConversionFactor = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    IsPerfect = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAlternateUoms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemAlternateUoms_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemAlternateUoms_UnitOfMeasures_UomId",
                        column: x => x.UomId,
                        principalTable: "UnitOfMeasures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemDrawings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    DrawingName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DrawingNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Revision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ShowInProductionBooking = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDrawings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemDrawings_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemTechnicalSpecs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    CapabilityType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CapabilityValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTechnicalSpecs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemTechnicalSpecs_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemWarehouseThresholds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    WarehouseName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MinThreshold = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MaxThreshold = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ReorderQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    OpeningQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    OpeningRate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    OpeningDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemWarehouseThresholds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemWarehouseThresholds_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemAlternateUoms_ItemId",
                table: "ItemAlternateUoms",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAlternateUoms_UomId",
                table: "ItemAlternateUoms",
                column: "UomId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDrawings_ItemId",
                table: "ItemDrawings",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_BaseUomId",
                table: "Items",
                column: "BaseUomId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Code",
                table: "Items",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemClassId",
                table: "Items",
                column: "ItemClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ParentItemId",
                table: "Items",
                column: "ParentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Sku",
                table: "Items",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemTechnicalSpecs_ItemId",
                table: "ItemTechnicalSpecs",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemWarehouseThresholds_ItemId",
                table: "ItemWarehouseThresholds",
                column: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemAlternateUoms");

            migrationBuilder.DropTable(
                name: "ItemDrawings");

            migrationBuilder.DropTable(
                name: "ItemTechnicalSpecs");

            migrationBuilder.DropTable(
                name: "ItemWarehouseThresholds");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
