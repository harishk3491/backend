using Items.Application.Common.Interfaces;
using Items.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Items.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ItemsDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IItemsDbContext>(sp => sp.GetRequiredService<ItemsDbContext>());
        services.AddScoped<IDbInitializer, DbInitializer>();

        return services;
    }
}
