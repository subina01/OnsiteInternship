using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Commands;

public class CancelOrderCommand : IRequest<Unit>
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
