using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace LibraryManagement.Domain.ValueObjects;


/// <summary>
/// Address Value Object - Represents a physical address
/// Implements DDD Value Object pattern - immutable and defined by its value
/// </summary>
public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    public string Country { get; private set; }

    private Address()
    {
        Street = string.Empty;
        City = string.Empty;
        State = string.Empty;
        ZipCode = string.Empty;
        Country = string.Empty;
    }

    private Address(
        string street,
        string city,
        string state,
        string zipCode,
        string country)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }

    public static Address Create(
        string street,
        string city,
        string state,
        string zipCode,
        string country)
    {
        Check.NotNullOrWhiteSpace(street, nameof(street));
        Check.NotNullOrWhiteSpace(city, nameof(city));
        Check.NotNullOrWhiteSpace(state, nameof(state));
        Check.NotNullOrWhiteSpace(zipCode, nameof(zipCode));
        Check.NotNullOrWhiteSpace(country, nameof(country));

        Check.Length(street, nameof(street), LibraryManagementConsts.Address.MaxStreetLength);
        Check.Length(city, nameof(city), LibraryManagementConsts.Address.MaxCityLength);
        Check.Length(state, nameof(state), LibraryManagementConsts.Address.MaxStateLength);
        Check.Length(zipCode, nameof(zipCode), LibraryManagementConsts.Address.MaxZipCodeLength);
        Check.Length(country, nameof(country), LibraryManagementConsts.Address.MaxCountryLength);

        return new Address(
            street.Trim(),
            city.Trim(),
            state.Trim(),
            zipCode.Trim(),
            country.Trim()
        );
    }

    public Address ChangeStreet(string street)
    {
        Check.NotNullOrWhiteSpace(street, nameof(street));
        Check.Length(street, nameof(street), LibraryManagementConsts.Address.MaxStreetLength);

        return new Address(street.Trim(), City, State, ZipCode, Country);
    }

    public Address ChangeCity(string city)
    {
        Check.NotNullOrWhiteSpace(city, nameof(city));
        Check.Length(city, nameof(city), LibraryManagementConsts.Address.MaxCityLength);

        return new Address(Street, city.Trim(), State, ZipCode, Country);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }

    public override string ToString()
    {
        return $"{Street}, {City}, {State} {ZipCode}, {Country}";
    }
}