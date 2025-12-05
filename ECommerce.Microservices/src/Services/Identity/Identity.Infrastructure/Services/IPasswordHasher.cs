using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Infrastructure.Services;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
