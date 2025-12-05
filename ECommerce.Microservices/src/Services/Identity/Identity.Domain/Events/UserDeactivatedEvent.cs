using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Events;

public class UserDeactivatedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public Guid UserId { get; }

    public UserDeactivatedEvent(Guid userId)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        UserId = userId;
    }
}
