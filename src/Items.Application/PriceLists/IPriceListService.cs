using Items.Application.Common;
using Items.Application.PriceLists.Dtos;

namespace Items.Application.PriceLists;

public interface IPriceListService
{
    Task<PagedResult<PriceListSummaryDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PriceListDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PriceListDto> CreateAsync(CreatePriceListRequest request, CancellationToken cancellationToken = default);
    Task<PriceListDto> UpdateAsync(int id, UpdatePriceListRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<PriceListDto> CloneAsync(int id, ClonePriceListRequest request, CancellationToken cancellationToken = default);
}
