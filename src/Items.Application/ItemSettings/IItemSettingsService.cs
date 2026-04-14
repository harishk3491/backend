using Items.Application.ItemSettings.Dtos;

namespace Items.Application.ItemSettings;

public interface IItemSettingsService
{
    Task<ItemSettingsDto> GetAsync(CancellationToken cancellationToken = default);
    Task<ItemSettingsDto> UpsertAsync(UpdateItemSettingsRequest request, CancellationToken cancellationToken = default);
}
