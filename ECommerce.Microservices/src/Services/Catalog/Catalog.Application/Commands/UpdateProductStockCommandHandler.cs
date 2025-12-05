using Catalog.Domain.Exceptions;
using Catalog.Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Commands;


public class UpdateProductStockCommandHandler : IRequestHandler<UpdateProductStockCommand, Unit>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductStockCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Unit> Handle(UpdateProductStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        product.UpdateStock(request.Quantity);

        await _productRepository.UpdateAsync(product, cancellationToken);

        return Unit.Value;
    }
}
