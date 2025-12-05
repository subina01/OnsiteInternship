using AutoMapper;
using MediatR;
using Order.Application.Dtos;
using Order.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Queries;

public class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, List<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrdersByCustomerQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetOrdersByCustomerAsync(request.CustomerId, cancellationToken);
        return _mapper.Map<List<OrderDto>>(orders);
    }
}
