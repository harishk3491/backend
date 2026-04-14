using Items.Application.Common;
using Items.Application.ItemClasses.Dtos;

namespace Items.Application.ItemClasses;

public interface IItemClassService
{
    Task<PagedResult<ItemClassSummaryDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ItemClassDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemClassDto> CreateAsync(CreateItemClassRequest request, CancellationToken cancellationToken = default);
    Task<ItemClassDto> UpdateAsync(int id, UpdateItemClassRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemClassDto> CloneAsync(int id, string newCode, string newName, CancellationToken cancellationToken = default);
}
