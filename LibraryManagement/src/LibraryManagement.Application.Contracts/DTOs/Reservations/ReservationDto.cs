using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace LibraryManagement.Application.Contracts.DTOs.Reservations;

public class ReservationDto : FullAuditedEntityDto<Guid>
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookISBN { get; set; } = string.Empty;

    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string MembershipNumber { get; set; } = string.Empty;

    public DateTime ReservationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public ReservationStatus Status { get; set; }
    public DateTime? ReadyForPickupDate { get; set; }
    public DateTime? FulfilledDate { get; set; }
    public string? Notes { get; set; }
    public bool IsExpired { get; set; }
}

