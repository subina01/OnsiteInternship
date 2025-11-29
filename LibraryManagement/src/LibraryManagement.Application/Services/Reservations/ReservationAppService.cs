using AutoMapper.Internal.Mappers;
using LibraryManagement.Application.Contracts.DTOs.Reservations;
using LibraryManagement.Application.Contracts.Permissions;
using LibraryManagement.Application.Contracts.Services;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using LibraryManagement.Domain.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Dynamic.Core;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using LibraryManagement.Domain.Shared.Enums;

namespace LibraryManagement.Application.Services.Reservations;

public class ReservationAppService : ApplicationService, IReservationAppService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRepository<Book, Guid> _bookRepository;
    private readonly IRepository<Member, Guid> _memberRepository;

    public ReservationAppService(
        IReservationRepository reservationRepository,
        IRepository<Book, Guid> bookRepository,
        IRepository<Member, Guid> memberRepository)
    {
        _reservationRepository = reservationRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
    }

    public async Task<ReservationDto> GetAsync(Guid id)
    {
        var reservation = await _reservationRepository.GetAsync(id, includeDetails: true);
        return ObjectMapper.Map<Reservation, ReservationDto>(reservation);
    }

    public async Task<PagedResultDto<ReservationDto>> GetListAsync(GetReservationListInput input)
    {
        var query = await _reservationRepository.WithDetailsAsync();

        query = query
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                x => x.Member.FirstName.Contains(input.Filter!) ||
                     x.Member.LastName.Contains(input.Filter!) ||
                     x.Book.Title.Contains(input.Filter!))
            .WhereIf(input.BookId.HasValue,
                x => x.BookId == input.BookId!.Value)
            .WhereIf(input.MemberId.HasValue,
                x => x.MemberId == input.MemberId!.Value)
            .WhereIf(input.Status.HasValue,
                x => x.Status == input.Status!.Value)
            .WhereIf(input.IsExpired == true,
                x => x.ExpirationDate < DateTime.UtcNow && x.Status != ReservationStatus.Fulfilled);

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = ApplySorting(query, input);
        query = ApplyPaging(query, input);

        var reservations = await AsyncExecuter.ToListAsync(query);
        var reservationDtos = ObjectMapper.Map<List<Reservation>, List<ReservationDto>>(reservations);

        return new PagedResultDto<ReservationDto>(totalCount, reservationDtos);
    }

    public async Task<ReservationDto> CreateAsync(CreateReservationDto input)
    {
        // Validate book exists
        var book = await _bookRepository.GetAsync(input.BookId);

        // Validate member exists
        var member = await _memberRepository.GetAsync(input.MemberId);

        // Check if member already has an active reservation for this book
        if (await _reservationRepository.HasActiveReservationAsync(input.BookId, input.MemberId))
        {
            throw new BusinessException(LibraryManagementErrorCodes.BookAlreadyReserved)
                .WithData("BookId", input.BookId)
                .WithData("MemberId", input.MemberId);
        }

        // Check if member has reached maximum reservation limit
        var activeReservationCount = await _reservationRepository.GetActiveReservationCountByMemberIdAsync(input.MemberId);
        if (activeReservationCount >= LibraryManagementConsts.Reservations.MaxActiveReservationsPerMember)
        {
            throw new BusinessException(LibraryManagementErrorCodes.MaximumReservationLimitReached)
                .WithData("MemberId", input.MemberId)
                .WithData("MaxReservations", LibraryManagementConsts.Reservations.MaxActiveReservationsPerMember);
        }

        var reservation = new Reservation(
            GuidGenerator.Create(),
            input.BookId,
            input.MemberId,
            DateTime.UtcNow);

        if (!input.Notes.IsNullOrWhiteSpace())
        {
            reservation.SetNotes(input.Notes);
        }

        await _reservationRepository.InsertAsync(reservation);

        return await GetAsync(reservation.Id);
    }

    public async Task<ReservationDto> UpdateAsync(Guid id, UpdateReservationDto input)
    {
        var reservation = await _reservationRepository.GetAsync(id);

        if (!input.Notes.IsNullOrWhiteSpace())
        {
            reservation.SetNotes(input.Notes);
        }

        await _reservationRepository.UpdateAsync(reservation);

        return await GetAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _reservationRepository.DeleteAsync(id);
    }

    public async Task<ReservationDto> MarkAsReadyAsync(Guid id)
    {
        var reservation = await _reservationRepository.GetAsync(id);
        reservation.MarkAsReady();
        await _reservationRepository.UpdateAsync(reservation);
        return await GetAsync(id);
    }

    public async Task<ReservationDto> FulfillAsync(Guid id)
    {
        var reservation = await _reservationRepository.GetAsync(id);
        reservation.Fulfill();
        await _reservationRepository.UpdateAsync(reservation);
        return await GetAsync(id);
    }

    public async Task<ReservationDto> CancelAsync(Guid id)
    {
        var reservation = await _reservationRepository.GetAsync(id);
        reservation.Cancel();
        await _reservationRepository.UpdateAsync(reservation);
        return await GetAsync(id);
    }

    public async Task<List<ReservationDto>> GetActiveReservationsAsync()
    {
        var reservations = await _reservationRepository.GetActiveReservationsAsync();
        return ObjectMapper.Map<List<Reservation>, List<ReservationDto>>(reservations);
    }

    public async Task<List<ReservationDto>> GetReservationsByMemberAsync(Guid memberId)
    {
        var reservations = await _reservationRepository.GetReservationsByMemberIdAsync(memberId);
        return ObjectMapper.Map<List<Reservation>, List<ReservationDto>>(reservations);
    }

    public async Task<List<ReservationDto>> GetReservationsByBookAsync(Guid bookId)
    {
        var reservations = await _reservationRepository.GetReservationsByBookIdAsync(bookId);
        return ObjectMapper.Map<List<Reservation>, List<ReservationDto>>(reservations);
    }

    public async Task UpdateExpiredReservationsAsync()
    {
        var expiredReservations = await _reservationRepository.GetExpiredReservationsAsync();

        foreach (var reservation in expiredReservations)
        {
            reservation.MarkAsExpired();
        }

        await _reservationRepository.UpdateManyAsync(expiredReservations);
    }

    protected virtual IQueryable<Reservation> ApplySorting(IQueryable<Reservation> query, GetReservationListInput input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            return query.OrderByDescending(x => x.ReservationDate);
        }

        // Use Dynamic LINQ to support string-based sorting
        return query.OrderBy(input.Sorting);
    }

    protected virtual IQueryable<Reservation> ApplyPaging(IQueryable<Reservation> query, GetReservationListInput input)
    {
        return query.Skip(input.SkipCount).Take(input.MaxResultCount);
    }
}
