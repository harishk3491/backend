using Items.Domain.Common;

namespace Items.Domain.Entities;

public class UnitOfMeasure : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
