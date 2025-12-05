using Identity.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Queries;

public class GetAllUsersQuery : IRequest<List<UserDto>>
{
}
