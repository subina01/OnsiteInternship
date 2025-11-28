using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories;

namespace LibraryManagement.Domain.Repositories;

/// <summary>
/// Repository interface for Reservation aggregate
/// </summary>
public interface IReservationRepository : IRepository<Reservation, Guid>
{
    Task<List<Reservation>> GetActiveReservationsAsync(
        CancellationToken cancellationToken = default);

    Task<List<Reservation>> GetExpiredReservationsAsync(
        CancellationToken cancellationToken = default);

    Task<List<Reservation>> GetReservationsByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default);

    Task<List<Reservation>> GetActiveReservationsByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default);

    Task<List<Reservation>> GetReservationsByBookIdAsync(
        Guid bookId,
        CancellationToken cancellationToken = default);

    Task<int> GetActiveReservationCountByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default);

    Task<bool> HasActiveReservationAsync(
        Guid bookId,
        Guid memberId,
        CancellationToken cancellationToken = default);
}
