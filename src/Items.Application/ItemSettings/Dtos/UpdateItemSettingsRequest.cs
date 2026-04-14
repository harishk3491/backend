using System.ComponentModel.DataAnnotations;
using Items.Domain.Enums;

namespace Items.Application.ItemSettings.Dtos;

public class UpdateItemSettingsRequest
{
    // Section 1: Core Identity & Governance
    [Range(1, 50)]
    public int ItemCodeMaxLength { get; set; } = 20;
    public bool AllowItemLevelCustomization { get; set; }
    public bool LogItemClassOverrides { get; set; }
    public bool AllowDuplicateNames { get; set; }
    public bool AllowTagNumberInSalesDocs { get; set; }

    // Section 2: Automated Tracking Logic
    public bool EnableAutoBatchSerialGeneration { get; set; }
    [MaxLength(50)]
    public string? AutoGenPrefix { get; set; }
    [MaxLength(50)]
    public string? AutoGenSuffix { get; set; }
    [MaxLength(10)]
    public string? AutoGenSeparator { get; set; }
    public int? AutoGenSequenceStart { get; set; }
    public int? AutoGenSequenceIncrement { get; set; }

    // Section 3: Copy & Duplication Rules
    public bool CopyDescription { get; set; }
    public bool CopyTechnicalSpecs { get; set; }
    public bool CopyPricingDetails { get; set; }
    public bool CopyTaxDetails { get; set; }
    public bool CopyItemClassDefaults { get; set; }
    public bool CopyAttachments { get; set; }

    // Section 4: ECN and CCN Settings
    public bool EcnEntryPermission { get; set; }
    public CcnNumberingBasis CcnNumberingBasis { get; set; }
}
