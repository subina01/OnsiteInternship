using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace LibraryManagement.Domain.ValueObjects;

/// <summary>
/// ISBN Value Object - Represents an International Standard Book Number
/// Implements DDD Value Object pattern - immutable and defined by its value
/// </summary>
public partial class ISBN : ValueObject
{
    public string Value { get; private set; }

    private ISBN()
    {
        Value = string.Empty;
    }

    private ISBN(string value)
    {
        Value = value;
    }

    public static ISBN Create(string value)
    {
        Check.NotNullOrWhiteSpace(value, nameof(value));

        var normalized = NormalizeIsbn(value);

        if (!IsValid(normalized))
        {
            throw new BusinessException(LibraryManagementErrorCodes.InvalidIsbn)
                .WithData("ISBN", value);
        }

        return new ISBN(normalized);
    }

    /// <summary>
    /// Normalizes ISBN by removing hyphens and spaces
    /// </summary>
    private static string NormalizeIsbn(string isbn)
    {
        return IsbnNormalizationRegex().Replace(isbn, string.Empty).ToUpperInvariant();
    }

    /// <summary>
    /// Validates ISBN-10 or ISBN-13 format
    /// </summary>
    public static bool IsValid(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            return false;

        var normalized = NormalizeIsbn(isbn);

        return normalized.Length switch
        {
            LibraryManagementConsts.Isbn10Length => IsValidIsbn10(normalized),
            LibraryManagementConsts.Isbn13Length => IsValidIsbn13(normalized),
            _ => false
        };
    }

    /// <summary>
    /// Validates ISBN-10 using checksum algorithm
    /// </summary>
    private static bool IsValidIsbn10(string isbn)
    {
        if (!Isbn10Regex().IsMatch(isbn))
            return false;

        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += (isbn[i] - '0') * (10 - i);
        }

        var checkDigit = isbn[9] == 'X' ? 10 : isbn[9] - '0';
        sum += checkDigit;

        return sum % 11 == 0;
    }

    /// <summary>
    /// Validates ISBN-13 using checksum algorithm
    /// </summary>
    private static bool IsValidIsbn13(string isbn)
    {
        if (!Isbn13Regex().IsMatch(isbn))
            return false;

        var sum = 0;
        for (var i = 0; i < 12; i++)
        {
            var digit = isbn[i] - '0';
            sum += i % 2 == 0 ? digit : digit * 3;
        }

        var checkDigit = isbn[12] - '0';
        var calculatedCheck = (10 - (sum % 10)) % 10;

        return checkDigit == calculatedCheck;
    }

    public bool IsIsbn10() => Value.Length == LibraryManagementConsts.Isbn10Length;
    public bool IsIsbn13() => Value.Length == LibraryManagementConsts.Isbn13Length;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    // Regex patterns using C# 11 generated regex for better performance
    [GeneratedRegex(@"[-\s]")]
    private static partial Regex IsbnNormalizationRegex();

    [GeneratedRegex(@"^\d{9}[\dX]$")]
    private static partial Regex Isbn10Regex();

    [GeneratedRegex(@"^\d{13}$")]
    private static partial Regex Isbn13Regex();
}
