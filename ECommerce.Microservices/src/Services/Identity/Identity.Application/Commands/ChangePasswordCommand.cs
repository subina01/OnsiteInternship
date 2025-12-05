using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Commands;

public class ChangePasswordCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
