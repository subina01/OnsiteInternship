using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Commands;

public class UpdateProductStockCommand : IRequest<Unit>
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
