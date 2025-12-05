using Basket.Domain.Exceptions;
using Basket.Infrastructure.EventBus;
using Basket.Infrastructure.Repositories;
using EventBus.RabbitMQ.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Application.Commands;

public class CheckoutBasketCommandHandler : IRequestHandler<CheckoutBasketCommand, bool>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IEventBus _eventBus;

    public CheckoutBasketCommandHandler(IBasketRepository basketRepository, IEventBus eventBus)
    {
        _basketRepository = basketRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(CheckoutBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.CustomerId, cancellationToken)
            ?? throw new BasketNotFoundException(request.CustomerId);

        if (!basket.Items.Any())
            throw new InvalidOperationException("Basket is empty");

        // Create integration event for Order service
        var checkoutEvent = new BasketCheckoutIntegrationEvent
        {
            CustomerId = request.CustomerId,
            UserName = request.UserName,
            TotalPrice = basket.TotalPrice,
            Street = request.Street,
            City = request.City,
            State = request.State,
            Country = request.Country,
            ZipCode = request.ZipCode,
            Items = basket.Items.Select(item => new BasketCheckoutItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList()
        };

        // Publish event to RabbitMQ for Order service
        await _eventBus.PublishAsync(checkoutEvent, cancellationToken);

        // Clear basket after checkout
        await _basketRepository.DeleteBasketAsync(request.CustomerId, cancellationToken);

        return true;
    }
}
