using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Events;

public class UserRegisteredEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; } = string.Empty;
    public string UserName { get; } = string.Empty;

    public UserRegisteredEvent(Guid userId, string email, string userName)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        UserId = userId;
        Email = email;
        UserName = userName;
    }
}

