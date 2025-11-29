using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Contracts.DTOs.Auth;


public class LoginDto
{
    public string UserNameOrEmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
