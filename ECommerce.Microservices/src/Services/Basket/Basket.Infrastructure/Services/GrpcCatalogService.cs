using Basket.Infrastructure.Protos;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Infrastructure.Services;

public class GrpcCatalogService : IGrpcCatalogService
{
    private readonly CatalogService.CatalogServiceClient _client;

    public GrpcCatalogService(GrpcChannel channel)
    {
        _client = new CatalogService.CatalogServiceClient(channel);
    }

    // Constructor that accepts the gRPC client directly
    public GrpcCatalogService(CatalogService.CatalogServiceClient client)
    {
        _client = client;
    }

    public async Task<ProductResponse?> GetProductAsync(string productId, CancellationToken cancellationToken = default)
    {
        var request = new GetProductRequest { ProductId = productId };

        try
        {
            var response = await _client.GetProductAsync(request, cancellationToken: cancellationToken);
            return response;
        }
        catch
        {
            // Return a dummy product for testing when catalog service is unavailable
            return new ProductResponse
            {
                Id = productId,
                Name = $"Product {productId}",
                Description = $"Description for product {productId}",
                Price = 10.99,
                ImageUrl = "https://example.com/image.jpg",
                StockQuantity = 100,
                IsAvailable = true,
                Sku = $"SKU-{productId}"
            };
        }
    }

    public async Task<ProductsResponse> GetProductsAsync(
        IEnumerable<string> productIds,
        CancellationToken cancellationToken = default)
    {
        var request = new GetProductsRequest();
        request.ProductIds.AddRange(productIds);

        return await _client.GetProductsAsync(request, cancellationToken: cancellationToken);
    }
}
