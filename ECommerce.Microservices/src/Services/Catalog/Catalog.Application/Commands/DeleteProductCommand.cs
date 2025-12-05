using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Commands;

public class DeleteProductCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
