using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagement.Application.Contracts.DTOs.Reservations;

public class CreateReservationDto
{
    [Required]
    public Guid BookId { get; set; }

    [Required]
    public Guid MemberId { get; set; }

    [StringLength(LibraryManagementConsts.MaxDescriptionLength)]
    public string? Notes { get; set; }
}
