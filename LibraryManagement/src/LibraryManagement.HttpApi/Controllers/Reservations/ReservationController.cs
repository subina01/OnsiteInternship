using Asp.Versioning;
using LibraryManagement.Application.Contracts.DTOs.Reservations;
using LibraryManagement.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace LibraryManagement.HttpApi.Controllers.Reservations;

[RemoteService(Name = "LibraryManagement")]
[Area("libraryManagement")]
[ControllerName("Reservation")]
[Route("api/library-management/reservations")]
public class ReservationController : AbpController
{
    private readonly IReservationAppService _reservationAppService;

    public ReservationController(IReservationAppService reservationAppService)
    {
        _reservationAppService = reservationAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<ReservationDto> GetAsync(Guid id)
    {
        return _reservationAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<ReservationDto>> GetListAsync(GetReservationListInput input)
    {
        return _reservationAppService.GetListAsync(input);
    }

    [HttpPost]
    public virtual Task<ReservationDto> CreateAsync(CreateReservationDto input)
    {
        return _reservationAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<ReservationDto> UpdateAsync(Guid id, UpdateReservationDto input)
    {
        return _reservationAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _reservationAppService.DeleteAsync(id);
    }

    [HttpPost]
    [Route("{id}/mark-as-ready")]
    public virtual Task<ReservationDto> MarkAsReadyAsync(Guid id)
    {
        return _reservationAppService.MarkAsReadyAsync(id);
    }

    [HttpPost]
    [Route("{id}/fulfill")]
    public virtual Task<ReservationDto> FulfillAsync(Guid id)
    {
        return _reservationAppService.FulfillAsync(id);
    }

    [HttpPost]
    [Route("{id}/cancel")]
    public virtual Task<ReservationDto> CancelAsync(Guid id)
    {
        return _reservationAppService.CancelAsync(id);
    }

    [HttpGet]
    [Route("active")]
    public virtual Task<List<ReservationDto>> GetActiveReservationsAsync()
    {
        return _reservationAppService.GetActiveReservationsAsync();
    }

    [HttpGet]
    [Route("by-member/{memberId}")]
    public virtual Task<List<ReservationDto>> GetReservationsByMemberAsync(Guid memberId)
    {
        return _reservationAppService.GetReservationsByMemberAsync(memberId);
    }

    [HttpGet]
    [Route("by-book/{bookId}")]
    public virtual Task<List<ReservationDto>> GetReservationsByBookAsync(Guid bookId)
    {
        return _reservationAppService.GetReservationsByBookAsync(bookId);
    }

    [HttpPost]
    [Route("update-expired")]
    public virtual Task UpdateExpiredReservationsAsync()
    {
        return _reservationAppService.UpdateExpiredReservationsAsync();
    }
}
