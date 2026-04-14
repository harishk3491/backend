using Items.Application.Common;
using Items.Application.ItemVendorMappings.Dtos;

namespace Items.Application.ItemVendorMappings;

public interface IItemVendorMappingService
{
    Task<PagedResult<ItemVendorMappingSummaryDto>> GetAllAsync(int page, int pageSize, int? itemId, int? vendorId, CancellationToken cancellationToken = default);
    Task<ItemVendorMappingDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemVendorMappingDto> CreateAsync(CreateItemVendorMappingRequest request, CancellationToken cancellationToken = default);
    Task<ItemVendorMappingDto> UpdateAsync(int id, UpdateItemVendorMappingRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemVendorMappingDto> CloneAsync(int id, CloneItemVendorMappingRequest request, CancellationToken cancellationToken = default);
}
