using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Caching;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // PostgreSQL Database
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("CatalogDb"),
                b => b.MigrationsAssembly(typeof(CatalogDbContext).Assembly.FullName)));

        // Redis Cache
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = ConfigurationOptions.Parse(
                sp.GetRequiredService<IConfiguration>().GetConnectionString("Redis")!,
                true);
            return ConnectionMultiplexer.Connect(configuration);
        });

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Caching
        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}
