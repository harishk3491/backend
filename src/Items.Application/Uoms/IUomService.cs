using Items.Application.Uoms.Dtos;

namespace Items.Application.Uoms;

public interface IUomService
{
    Task<List<UomDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UomDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UomDto> CreateAsync(CreateUomRequest request, CancellationToken cancellationToken = default);
    Task<UomDto> UpdateAsync(int id, UpdateUomRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
