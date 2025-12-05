using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Commands;

public class DeleteCategoryCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
