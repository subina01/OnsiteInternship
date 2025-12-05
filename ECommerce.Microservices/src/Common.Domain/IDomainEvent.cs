using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Domain;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}
