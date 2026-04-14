using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Items.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddItemSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCodeMaxLength = table.Column<int>(type: "int", nullable: false, defaultValue: 20),
                    AllowItemLevelCustomization = table.Column<bool>(type: "bit", nullable: false),
                    LogItemClassOverrides = table.Column<bool>(type: "bit", nullable: false),
                    AllowDuplicateNames = table.Column<bool>(type: "bit", nullable: false),
                    AllowTagNumberInSalesDocs = table.Column<bool>(type: "bit", nullable: false),
                    EnableAutoBatchSerialGeneration = table.Column<bool>(type: "bit", nullable: false),
                    AutoGenPrefix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AutoGenSuffix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AutoGenSeparator = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AutoGenSequenceStart = table.Column<int>(type: "int", nullable: true),
                    AutoGenSequenceIncrement = table.Column<int>(type: "int", nullable: true),
                    CopyDescription = table.Column<bool>(type: "bit", nullable: false),
                    CopyTechnicalSpecs = table.Column<bool>(type: "bit", nullable: false),
                    CopyPricingDetails = table.Column<bool>(type: "bit", nullable: false),
                    CopyTaxDetails = table.Column<bool>(type: "bit", nullable: false),
                    CopyItemClassDefaults = table.Column<bool>(type: "bit", nullable: false),
                    CopyAttachments = table.Column<bool>(type: "bit", nullable: false),
                    EcnEntryPermission = table.Column<bool>(type: "bit", nullable: false),
                    CcnNumberingBasis = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemSettings");
        }
    }
}
