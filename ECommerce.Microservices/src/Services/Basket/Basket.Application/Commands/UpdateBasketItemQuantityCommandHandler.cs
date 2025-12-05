using Basket.Domain.Exceptions;
using Basket.Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Application.Commands;

public class UpdateBasketItemQuantityCommandHandler : IRequestHandler<UpdateBasketItemQuantityCommand, Unit>
{
    private readonly IBasketRepository _basketRepository;

    public UpdateBasketItemQuantityCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Unit> Handle(UpdateBasketItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.CustomerId, cancellationToken)
            ?? throw new BasketNotFoundException(request.CustomerId);

        basket.UpdateItemQuantity(request.ProductId, request.Quantity);

        await _basketRepository.CreateOrUpdateBasketAsync(basket, cancellationToken);

        return Unit.Value;
    }
}
