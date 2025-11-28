using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagement.Application.Contracts.DTOs.Reservations;

public class UpdateReservationDto
{
    [StringLength(LibraryManagementConsts.MaxDescriptionLength)]
    public string? Notes { get; set; }
}
