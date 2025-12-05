using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Commands;

public class UpdateOrderStatusCommand : IRequest<Unit>
{
    public Guid OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }
}
