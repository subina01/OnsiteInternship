using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Commands;

public class RegisterUserCommand : IRequest<Guid>
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}
