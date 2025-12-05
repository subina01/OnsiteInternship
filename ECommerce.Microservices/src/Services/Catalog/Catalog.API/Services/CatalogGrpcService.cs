using Catalog.API.Protos;
using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Catalog.API.Services;

public class CatalogGrpcService : CatalogService.CatalogServiceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CatalogGrpcService> _logger;

    public CatalogGrpcService(IMediator mediator, ILogger<CatalogGrpcService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override async Task<ProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC GetProduct called for ProductId: {ProductId}", request.ProductId);

        if (!Guid.TryParse(request.ProductId, out var productId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid product ID format"));
        }

        var query = new GetProductByIdQuery { Id = productId };
        var product = await _mediator.Send(query);

        if (product == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Product {request.ProductId} not found"));
        }

        return new ProductResponse
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Description = product.Description,
            Price = (double)product.Price,
            ImageUrl = product.ImageUrl,
            StockQuantity = product.StockQuantity,
            IsAvailable = product.IsAvailable,
            Sku = product.SKU
        };
    }

    public override async Task<ProductsResponse> GetProducts(GetProductsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC GetProducts called for {Count} products", request.ProductIds.Count);

        var response = new ProductsResponse();

        foreach (var productId in request.ProductIds)
        {
            if (!Guid.TryParse(productId, out var id))
                continue;

            var query = new GetProductByIdQuery { Id = id };
            var product = await _mediator.Send(query);

            if (product != null)
            {
                response.Products.Add(new ProductResponse
                {
                    Id = product.Id.ToString(),
                    Name = product.Name,
                    Description = product.Description,
                    Price = (double)product.Price,
                    ImageUrl = product.ImageUrl,
                    StockQuantity = product.StockQuantity,
                    IsAvailable = product.IsAvailable,
                    Sku = product.SKU
                });
            }
        }

        return response;
    }

    public override async Task<UpdateStockResponse> UpdateStock(UpdateStockRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC UpdateStock called for ProductId: {ProductId}, Quantity: {Quantity}",
            request.ProductId, request.Quantity);

        try
        {
            if (!Guid.TryParse(request.ProductId, out var productId))
            {
                return new UpdateStockResponse
                {
                    Success = false,
                    Message = "Invalid product ID format"
                };
            }

            var command = new UpdateProductStockCommand
            {
                ProductId = productId,
                Quantity = request.Quantity
            };

            await _mediator.Send(command);

            return new UpdateStockResponse
            {
                Success = true,
                Message = "Stock updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for product {ProductId}", request.ProductId);
            return new UpdateStockResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}
