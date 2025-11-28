using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagement.Application.Contracts.DTOs.Members;

public class CreateUpdateAddressDto
{
    [Required]
    [StringLength(LibraryManagementConsts.Address.MaxStreetLength)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [StringLength(LibraryManagementConsts.Address.MaxCityLength)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(LibraryManagementConsts.Address.MaxStateLength)]
    public string State { get; set; } = string.Empty;

    [Required]
    [StringLength(LibraryManagementConsts.Address.MaxZipCodeLength)]
    public string ZipCode { get; set; } = string.Empty;

    [Required]
    [StringLength(LibraryManagementConsts.Address.MaxCountryLength)]
    public string Country { get; set; } = string.Empty;
}
