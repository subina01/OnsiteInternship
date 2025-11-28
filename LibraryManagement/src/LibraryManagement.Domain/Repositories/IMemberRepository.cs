using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories;

namespace LibraryManagement.Domain.Repositories;

/// <summary>
/// Repository interface for Member aggregate
/// </summary>
public interface IMemberRepository : IRepository<Member, Guid>
{
    Task<Member?> FindByMembershipNumberAsync(
        string membershipNumber,
        CancellationToken cancellationToken = default);

    Task<Member?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<List<Member>> GetExpiredMembersAsync(
        CancellationToken cancellationToken = default);

    Task<List<Member>> GetActiveMembersAsync(
        CancellationToken cancellationToken = default);

    Task<bool> IsMembershipNumberExistsAsync(
        string membershipNumber,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> IsEmailExistsAsync(
        string email,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
}
