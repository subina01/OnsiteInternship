using MediatR;
using Order.Domain.Exceptions;
using Order.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Commands;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Unit> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        switch (request.Status.ToLower())
        {
            case "paid":
                order.SetPaidStatus();
                break;
            case "shipped":
                if (string.IsNullOrEmpty(request.TrackingNumber))
                    throw new InvalidOperationException("Tracking number is required for shipped status");
                order.SetShippedStatus(request.TrackingNumber);
                break;
            case "delivered":
                order.SetDeliveredStatus();
                break;
            default:
                throw new InvalidOperationException($"Invalid order status: {request.Status}");
        }

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return Unit.Value;
    }
}
