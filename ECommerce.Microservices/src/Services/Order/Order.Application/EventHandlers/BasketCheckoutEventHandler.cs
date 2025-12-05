using Basket.Infrastructure.EventBus;
using EventBus.RabbitMQ.Abstractions;
using MediatR;
using Order.Application.Commands;
using Order.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Order.Application.EventHandlers;

public class BasketCheckoutEventHandler : IIntegrationEventHandler<BasketCheckoutIntegrationEvent>
{
    private readonly IMediator _mediator;

    public BasketCheckoutEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(BasketCheckoutIntegrationEvent @event)
    {
        var command = new CreateOrderCommand
        {
            CustomerId = @event.CustomerId,
            ShippingAddress = new AddressDto
            {
                Street = @event.Street,
                City = @event.City,
                State = @event.State,
                Country = @event.Country,
                ZipCode = @event.ZipCode
            },
            BillingAddress = new AddressDto
            {
                Street = @event.Street,
                City = @event.City,
                State = @event.State,
                Country = @event.Country,
                ZipCode = @event.ZipCode
            },
            Items = @event.Items.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList()
        };

        await _mediator.Send(command);
    }
}
