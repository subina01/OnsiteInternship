using Basket.Domain.Exceptions;
using Basket.Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Application.Commands;

public class RemoveItemFromBasketCommandHandler : IRequestHandler<RemoveItemFromBasketCommand, Unit>
{
    private readonly IBasketRepository _basketRepository;

    public RemoveItemFromBasketCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Unit> Handle(RemoveItemFromBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.CustomerId, cancellationToken)
            ?? throw new BasketNotFoundException(request.CustomerId);

        basket.RemoveItem(request.ProductId);

        await _basketRepository.CreateOrUpdateBasketAsync(basket, cancellationToken);

        return Unit.Value;
    }
}
