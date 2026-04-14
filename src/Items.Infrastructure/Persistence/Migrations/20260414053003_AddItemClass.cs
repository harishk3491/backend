using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Items.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddItemClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ItemNature = table.Column<int>(type: "int", nullable: false),
                    ItemType = table.Column<int>(type: "int", nullable: true),
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
                    BomQtyInWeightLengthWithDimensions = table.Column<bool>(type: "bit", nullable: false),
                    NeedDimensionWiseStockKeeping = table.Column<bool>(type: "bit", nullable: false),
                    NeedDimensionWiseConsumptionInBom = table.Column<bool>(type: "bit", nullable: false),
                    LongItem = table.Column<bool>(type: "bit", nullable: false),
                    BomQtyInDimensionsAndPieces = table.Column<bool>(type: "bit", nullable: false),
                    SalesAccountGroup = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SalesAccount = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PurchaseAccountGroup = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PurchaseAccount = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemClasses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemClasses_Code",
                table: "ItemClasses",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemClasses");
        }
    }
}
