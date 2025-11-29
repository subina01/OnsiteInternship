using LibraryManagement.Application.Contracts.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;

namespace LibraryManagement.HttpApi.Controllers.Auth;

[Route("api/Auth")]
[ApiController]
public class AuthController : AbpController
{
    private readonly IIdentityUserRepository _userRepository;
    private readonly IPasswordHasher<Volo.Abp.Identity.IdentityUser> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthController(
        IIdentityUserRepository userRepository,
        IPasswordHasher<Volo.Abp.Identity.IdentityUser> passwordHasher,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto input)
    {
        var user = await _userRepository.FindByNormalizedUserNameAsync(input.UserNameOrEmailAddress.ToUpperInvariant())
                   ?? await _userRepository.FindByNormalizedEmailAsync(input.UserNameOrEmailAddress.ToUpperInvariant());

        if (user == null)
            return Unauthorized(new { message = "Invalid credentials" });

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            input.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Invalid credentials" });

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim("userId", user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"]!),
        new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]!)
    };

        var secretKey = _configuration["Jwt:SecretKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}