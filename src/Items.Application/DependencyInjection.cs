using FluentValidation;
using Items.Application.ItemClasses;
using Items.Application.Items;
using Items.Application.ItemSettings;
using Items.Application.ItemVendorMappings;
using Items.Application.PriceLists;
using Items.Application.Uoms;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Items.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IItemSettingsService, ItemSettingsService>();
        services.AddScoped<IUomService, UomService>();
        services.AddScoped<IItemClassService, ItemClassService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IItemVendorMappingService, ItemVendorMappingService>();
        services.AddScoped<IPriceListService, PriceListService>();
        return services;
    }
}
