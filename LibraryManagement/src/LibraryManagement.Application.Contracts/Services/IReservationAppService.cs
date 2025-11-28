using LibraryManagement.Application.Contracts.DTOs.Reservations;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LibraryManagement.Application.Contracts.Services;

public interface IReservationAppService : IApplicationService
{
    Task<ReservationDto> GetAsync(Guid id);
    Task<PagedResultDto<ReservationDto>> GetListAsync(GetReservationListInput input);
    Task<ReservationDto> CreateAsync(CreateReservationDto input);
    Task<ReservationDto> UpdateAsync(Guid id, UpdateReservationDto input);
    Task DeleteAsync(Guid id);
    Task<ReservationDto> MarkAsReadyAsync(Guid id);
    Task<ReservationDto> FulfillAsync(Guid id);
    Task<ReservationDto> CancelAsync(Guid id);
    Task<List<ReservationDto>> GetActiveReservationsAsync();
    Task<List<ReservationDto>> GetReservationsByMemberAsync(Guid memberId);
    Task<List<ReservationDto>> GetReservationsByBookAsync(Guid bookId);
    Task UpdateExpiredReservationsAsync();
}
