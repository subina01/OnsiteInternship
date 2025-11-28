using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace LibraryManagement.Application.Contracts.DTOs.Reservations;

public class GetReservationListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? BookId { get; set; }
    public Guid? MemberId { get; set; }
    public ReservationStatus? Status { get; set; }
    public bool? IsExpired { get; set; }
}
