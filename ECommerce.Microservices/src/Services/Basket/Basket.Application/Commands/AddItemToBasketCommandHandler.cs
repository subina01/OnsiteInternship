using Basket.Domain.Entities;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Application.Commands;

public class AddItemToBasketCommandHandler : IRequestHandler<AddItemToBasketCommand, Unit>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IGrpcCatalogService _catalogService;
    private readonly ILogger<AddItemToBasketCommandHandler> _logger;

    public AddItemToBasketCommandHandler(
        IBasketRepository basketRepository,
        IGrpcCatalogService catalogService,
        ILogger<AddItemToBasketCommandHandler> logger)
    {
        _basketRepository = basketRepository;
        _catalogService = catalogService;
        _logger = logger;
    }

    public async Task<Unit> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product {ProductId} from Catalog service", request.ProductId);

        // Get product details from Catalog service via gRPC
        var product = await _catalogService.GetProductAsync(request.ProductId.ToString(), cancellationToken);

        if (product is null)
        {
            _logger.LogError("Product {ProductId} not found in Catalog service", request.ProductId);
            throw new InvalidOperationException($"Product {request.ProductId} not found");
        }

        _logger.LogInformation("Found product {ProductId}: {ProductName}", request.ProductId, product.Name);

        // Get or create basket
        var basket = await _basketRepository.GetBasketAsync(request.CustomerId, cancellationToken)
                     ?? new CustomerBasket(request.CustomerId);

        _logger.LogInformation("Basket for customer {CustomerId} has {ItemCount} items before adding",
            request.CustomerId, basket.Items.Count);

        // Add item to basket
        basket.AddItem(
            request.ProductId,
            product.Name,
            (decimal)product.Price,
            request.Quantity,
            product.ImageUrl
        );

        _logger.LogInformation("Added item to basket. Basket now has {ItemCount} items", basket.Items.Count);

        // Save basket
        var savedBasket = await _basketRepository.CreateOrUpdateBasketAsync(basket, cancellationToken);

        _logger.LogInformation("Basket saved successfully for customer {CustomerId} with {ItemCount} items",
            request.CustomerId, savedBasket.Items.Count);

        return Unit.Value;
    }
}
