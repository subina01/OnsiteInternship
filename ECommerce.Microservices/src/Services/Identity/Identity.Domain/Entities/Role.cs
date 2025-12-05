using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Entities;

public class Role : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private Role() { } // EF Core

    public Role(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty", nameof(name));

        Name = name;
        Description = description;
    }

    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty", nameof(name));

        Name = name;
        Description = description;
    }

    // Predefined roles
    public static Role Administrator => new(RoleNames.Administrator, "Full system access");
    public static Role Customer => new(RoleNames.Customer, "Regular customer access");
    public static Role Manager => new(RoleNames.Manager, "Management access");
}

public static class RoleNames
{
    public const string Administrator = "Administrator";
    public const string Customer = "Customer";
    public const string Manager = "Manager";
}
