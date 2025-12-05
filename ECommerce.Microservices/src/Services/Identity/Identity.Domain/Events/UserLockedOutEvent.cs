using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Events;

public class UserLockedOutEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public DateTime LockoutEnd { get; }

    public UserLockedOutEvent(Guid userId, DateTime lockoutEnd)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        UserId = userId;
        LockoutEnd = lockoutEnd;
    }
}
