using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Exceptions;

public class InvalidCredentialsException : IdentityDomainException
{
    public InvalidCredentialsException()
        : base("Invalid email or password.")
    { }
}
