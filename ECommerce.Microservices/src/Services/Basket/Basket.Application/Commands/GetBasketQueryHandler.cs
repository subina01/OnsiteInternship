using AutoMapper;
using Basket.Application.DTOs;
using Basket.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Application.Commands;

public class GetBasketQueryHandler : IRequestHandler<GetBasketQuery, BasketDto?>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetBasketQueryHandler> _logger;

    public GetBasketQueryHandler(IBasketRepository basketRepository, IMapper mapper, ILogger<GetBasketQueryHandler> logger)
    {
        _basketRepository = basketRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BasketDto?> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting basket for customer {CustomerId}", request.CustomerId);

        var basket = await _basketRepository.GetBasketAsync(request.CustomerId, cancellationToken);

        if (basket == null)
        {
            _logger.LogInformation("No basket found for customer {CustomerId}", request.CustomerId);
            return null;
        }

        _logger.LogInformation("Found basket for customer {CustomerId} with {ItemCount} items",
            request.CustomerId, basket.Items.Count);

        return _mapper.Map<BasketDto>(basket);
    }
}
