using Basket.Infrastructure.Protos;

namespace Basket.Infrastructure.Services;

public interface IGrpcCatalogService
{
    Task<ProductResponse?> GetProductAsync(string productId, CancellationToken cancellationToken = default);
    Task<ProductsResponse> GetProductsAsync(IEnumerable<string> productIds, CancellationToken cancellationToken = default);
}
