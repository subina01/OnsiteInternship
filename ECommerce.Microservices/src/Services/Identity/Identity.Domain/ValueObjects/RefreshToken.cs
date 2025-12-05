using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.ValueObjects;

public class RefreshToken : BaseValueObject
{
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiryDate { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedDate { get; private set; }
    public Guid UserId { get; private set; }

    private RefreshToken() { } // EF Core

    public RefreshToken(string token, DateTime expiryDate, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty", nameof(token));

        Token = token;
        ExpiryDate = expiryDate;
        UserId = userId;
        IsRevoked = false;
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiryDate;

    public bool IsActive() => !IsRevoked && !IsExpired();

    public void Revoke()
    {
        IsRevoked = true;
        RevokedDate = DateTime.UtcNow;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Token;
        yield return ExpiryDate;
        yield return UserId;
    }
}

