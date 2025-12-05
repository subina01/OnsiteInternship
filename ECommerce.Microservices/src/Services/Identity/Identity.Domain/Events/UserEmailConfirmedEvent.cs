using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Events;

public class UserEmailConfirmedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }

    public UserEmailConfirmedEvent(Guid userId, string email)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        UserId = userId;
        Email = email;
    }
}
