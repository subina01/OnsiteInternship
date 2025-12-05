using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Exceptions;

public class UserNotFoundException : IdentityDomainException
{
    public UserNotFoundException(Guid userId)
        : base($"User with id {userId} was not found.")
    { }

    public UserNotFoundException(string email)
        : base($"User with email {email} was not found.")
    { }
}
