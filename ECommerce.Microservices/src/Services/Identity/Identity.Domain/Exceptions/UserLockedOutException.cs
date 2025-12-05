using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Exceptions;

public class UserLockedOutException : IdentityDomainException
{
    public UserLockedOutException(DateTime lockoutEnd)
        : base($"User account is locked until {lockoutEnd:yyyy-MM-dd HH:mm:ss} UTC.")
    { }
}
