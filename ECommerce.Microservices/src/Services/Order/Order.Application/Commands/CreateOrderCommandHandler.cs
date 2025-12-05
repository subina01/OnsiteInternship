using MediatR;
using Order.Domain.ValueObjects;
using Order.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var shippingAddress = new Address(
            request.ShippingAddress.Street,
            request.ShippingAddress.City,
            request.ShippingAddress.State,
            request.ShippingAddress.Country,
            request.ShippingAddress.ZipCode
        );

        var billingAddress = new Address(
            request.BillingAddress.Street,
            request.BillingAddress.City,
            request.BillingAddress.State,
            request.BillingAddress.Country,
            request.BillingAddress.ZipCode
        );

        var order = new Domain.Entities.Order(
            request.CustomerId,
            shippingAddress,
            billingAddress
        );

        foreach (var item in request.Items)
        {
            order.AddOrderItem(item.ProductId, item.ProductName, item.Price, item.Quantity);
        }

        await _orderRepository.AddAsync(order, cancellationToken);

        return order.Id;
    }
}
