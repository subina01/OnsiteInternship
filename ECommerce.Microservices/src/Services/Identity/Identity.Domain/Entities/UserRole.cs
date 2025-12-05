using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; } = null!;
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    private UserRole() { } // EF Core

    public UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
