using Catalog.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Queries;

public class GetProductsByCategoryQuery : IRequest<List<ProductDto>>
{
    public Guid CategoryId { get; set; }
}
