using Basket.Infrastructure.Protos;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Services;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // --- Redis Cache for Basket Storage ---
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisConfiguration = ConfigurationOptions.Parse(
                configuration.GetConnectionString("Redis")!,
                true);
            return ConnectionMultiplexer.Connect(redisConfiguration);
        });

        // --- Repositories ---
        services.AddScoped<IBasketRepository, BasketRepository>();

        // --- gRPC Client Configuration ---

        // Register the concrete generated gRPC client using the client factory.
        services.AddGrpcClient<CatalogService.CatalogServiceClient>(options =>
        {
            var catalogGrpcUrl = configuration["GrpcSettings:CatalogUrl"]
                                ?? "https://localhost:7029";

            // The factory manages channel creation internally
            options.Address = new Uri(catalogGrpcUrl);
        });

        // Register the wrapper service that implements IGrpcCatalogService
        // Use the injected gRPC client directly
        services.AddScoped<IGrpcCatalogService>(sp =>
        {
            var grpcClient = sp.GetRequiredService<CatalogService.CatalogServiceClient>();
            return new GrpcCatalogService(grpcClient);
        });

        return services;
    }
}
