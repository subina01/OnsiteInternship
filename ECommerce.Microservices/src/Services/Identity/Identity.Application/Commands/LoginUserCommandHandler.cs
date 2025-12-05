using AutoMapper;
using Identity.Application.DTOs;
using Identity.Domain.Exceptions;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Commands;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IMapper mapper,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // Get user
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new InvalidCredentialsException();

        // Check if user is locked out
        if (user.IsLockedOut())
            throw new UserLockedOutException(user.LockoutEnd!.Value);

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin(
                maxAttempts: Convert.ToInt32(_configuration["Security:MaxLoginAttempts"] ?? "5"),
                lockoutMinutes: Convert.ToInt32(_configuration["Security:LockoutMinutes"] ?? "15")
            );
            await _userRepository.UpdateAsync(user, cancellationToken);
            throw new InvalidCredentialsException();
        }

        // Check if user is active
        if (!user.IsActive)
            throw new InvalidOperationException("User account is deactivated");

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(
            Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryInDays"] ?? "7"));
        user.AddRefreshToken(refreshToken, refreshTokenExpiry);

        // Record successful login
        user.RecordSuccessfulLogin();

        await _userRepository.UpdateAsync(user, cancellationToken);

        var userDto = _mapper.Map<UserDto>(user);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = userDto,
            ExpiresAt = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"] ?? "60"))
        };
    }
}
