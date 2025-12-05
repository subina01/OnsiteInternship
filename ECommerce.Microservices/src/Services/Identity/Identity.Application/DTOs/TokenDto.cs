using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.DTOs;

public class TokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
