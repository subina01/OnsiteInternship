using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.EntityFrameworkCore.EntityFrameworkCore;

namespace LibraryManagement.EntityFrameworkCore.Repositories;

public class EfCoreReservationRepository : EfCoreRepository<LibraryManagementDbContext, Reservation, Guid>, IReservationRepository
{
    public EfCoreReservationRepository(IDbContextProvider<LibraryManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Reservation>> GetActiveReservationsAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Status == ReservationStatus.Pending || x.Status == ReservationStatus.Ready)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Reservation>> GetExpiredReservationsAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var now = DateTime.UtcNow;
        return await dbSet
            .Where(x => (x.Status == ReservationStatus.Pending || x.Status == ReservationStatus.Ready)
                       && x.ExpirationDate < now)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Reservation>> GetReservationsByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.MemberId == memberId)
            .OrderByDescending(x => x.ReservationDate)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Reservation>> GetActiveReservationsByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.MemberId == memberId
                       && (x.Status == ReservationStatus.Pending || x.Status == ReservationStatus.Ready))
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Reservation>> GetReservationsByBookIdAsync(
        Guid bookId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.BookId == bookId)
            .OrderByDescending(x => x.ReservationDate)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<int> GetActiveReservationCountByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .CountAsync(
                x => x.MemberId == memberId
                    && (x.Status == ReservationStatus.Pending || x.Status == ReservationStatus.Ready),
                GetCancellationToken(cancellationToken));
    }

    public async Task<bool> HasActiveReservationAsync(
        Guid bookId,
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AnyAsync(
                x => x.BookId == bookId
                    && x.MemberId == memberId
                    && (x.Status == ReservationStatus.Pending || x.Status == ReservationStatus.Ready),
                GetCancellationToken(cancellationToken));
    }

    public override async Task<IQueryable<Reservation>> WithDetailsAsync()
    {
        return (await GetQueryableAsync())
            .Include(x => x.Book)
            .Include(x => x.Member);
    }
}