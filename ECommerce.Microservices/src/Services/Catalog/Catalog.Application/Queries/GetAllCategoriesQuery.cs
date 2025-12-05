using Catalog.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Queries;

public class GetAllCategoriesQuery : IRequest<List<CategoryDto>>
{
}
