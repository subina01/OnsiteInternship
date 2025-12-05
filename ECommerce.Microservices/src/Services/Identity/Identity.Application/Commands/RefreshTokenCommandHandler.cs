using Identity.Application.DTOs;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<TokenDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Get user via abstraction (no EF Core here)
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user == null)
            throw new InvalidOperationException("Invalid refresh token");

        var refreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken);

        if (refreshToken == null || !refreshToken.IsActive())
            throw new InvalidOperationException("Invalid or expired refresh token");

        // Generate new tokens
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Revoke old refresh token
        user.RevokeRefreshToken(request.RefreshToken);

        // Add new refresh token
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(
            Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryInDays"] ?? "7"));
        user.AddRefreshToken(newRefreshToken, refreshTokenExpiry);

        // Save changes
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new TokenDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}

