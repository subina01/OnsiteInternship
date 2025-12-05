using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ.Events;

public abstract record IntegrationEvent
{
    public Guid Id { get; init; }
    public DateTime CreatedDate { get; init; }

    protected IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
    }
}
