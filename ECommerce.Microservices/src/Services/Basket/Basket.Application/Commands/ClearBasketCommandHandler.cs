using Basket.Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Application.Commands;

public class ClearBasketCommandHandler : IRequestHandler<ClearBasketCommand, Unit>
{
    private readonly IBasketRepository _basketRepository;

    public ClearBasketCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Unit> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        await _basketRepository.DeleteBasketAsync(request.CustomerId, cancellationToken);
        return Unit.Value;
    }
}
