using Items.Application.Common;
using Items.Application.Items.Dtos;
using Items.Domain.Enums;

namespace Items.Application.Items;

public interface IItemService
{
    Task<PagedResult<ItemSummaryDto>> GetAllAsync(int page, int pageSize, int? itemClassId, ItemType? itemType, ItemNature? itemNature, CancellationToken cancellationToken = default);
    Task<ItemDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemDto> CreateAsync(CreateItemRequest request, CancellationToken cancellationToken = default);
    Task<ItemDto> UpdateAsync(int id, UpdateItemRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemDto> CloneAsync(int id, CloneItemRequest request, CancellationToken cancellationToken = default);
}
