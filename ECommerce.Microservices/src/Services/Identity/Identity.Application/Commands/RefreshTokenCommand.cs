using Identity.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Commands;

public class RefreshTokenCommand : IRequest<TokenDto>
{
    public string RefreshToken { get; set; } = string.Empty;
}
