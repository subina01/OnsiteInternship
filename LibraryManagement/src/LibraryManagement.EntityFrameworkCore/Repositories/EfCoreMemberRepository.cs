using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.EntityFrameworkCore.EntityFrameworkCore;

namespace LibraryManagement.EntityFrameworkCore.Repositories;

public class EfCoreMemberRepository : EfCoreRepository<LibraryManagementDbContext, Member, Guid>, IMemberRepository
{
    public EfCoreMemberRepository(IDbContextProvider<LibraryManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Member?> FindByMembershipNumberAsync(
        string membershipNumber,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(
                x => x.MembershipNumber == membershipNumber,
                GetCancellationToken(cancellationToken));
    }

    public async Task<Member?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(
                x => x.Email == email,
                GetCancellationToken(cancellationToken));
    }

    public async Task<List<Member>> GetExpiredMembersAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var now = DateTime.UtcNow;
        return await dbSet
            .Where(x => x.ExpiryDate < now)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Member>> GetActiveMembersAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var now = DateTime.UtcNow;
        return await dbSet
            .Where(x => x.ExpiryDate >= now)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<bool> IsMembershipNumberExistsAsync(
        string membershipNumber,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.Where(x => x.MembershipNumber == membershipNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<bool> IsEmailExistsAsync(
        string email,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.Where(x => x.Email == email);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }
}
