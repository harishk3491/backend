using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Items.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddItemVendorMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemVendorMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    VendorItemCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VendorSku = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsPreferredVendor = table.Column<bool>(type: "bit", nullable: false),
                    Moq = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MaxOrderQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Eoq = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    StandardLeadTimeDays = table.Column<int>(type: "int", nullable: false),
                    WarrantyDuration = table.Column<int>(type: "int", nullable: true),
                    WarrantyDurationUnit = table.Column<int>(type: "int", nullable: true),
                    WarrantyStartBasis = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemVendorMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemVendorMappings_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemVendorPricings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemVendorMappingId = table.Column<int>(type: "int", nullable: false),
                    BasePurchasePrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: true),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    EffectivePurchasePrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PriceValidFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    PriceValidTo = table.Column<DateOnly>(type: "date", nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemVendorPricings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemVendorPricings_ItemVendorMappings_ItemVendorMappingId",
                        column: x => x.ItemVendorMappingId,
                        principalTable: "ItemVendorMappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemVendorPurchaseUoms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemVendorMappingId = table.Column<int>(type: "int", nullable: false),
                    PurchaseUomId = table.Column<int>(type: "int", nullable: false),
                    IsPerfectConversion = table.Column<bool>(type: "bit", nullable: false),
                    ConversionRate = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TolerancePercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemVendorPurchaseUoms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemVendorPurchaseUoms_ItemVendorMappings_ItemVendorMappingId",
                        column: x => x.ItemVendorMappingId,
                        principalTable: "ItemVendorMappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemVendorPurchaseUoms_UnitOfMeasures_PurchaseUomId",
                        column: x => x.PurchaseUomId,
                        principalTable: "UnitOfMeasures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemVendorMappings_Code",
                table: "ItemVendorMappings",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemVendorMappings_ItemId_VendorId",
                table: "ItemVendorMappings",
                columns: new[] { "ItemId", "VendorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemVendorPricings_ItemVendorMappingId",
                table: "ItemVendorPricings",
                column: "ItemVendorMappingId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemVendorPurchaseUoms_ItemVendorMappingId",
                table: "ItemVendorPurchaseUoms",
                column: "ItemVendorMappingId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemVendorPurchaseUoms_PurchaseUomId",
                table: "ItemVendorPurchaseUoms",
                column: "PurchaseUomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemVendorPricings");

            migrationBuilder.DropTable(
                name: "ItemVendorPurchaseUoms");

            migrationBuilder.DropTable(
                name: "ItemVendorMappings");
        }
    }
}
