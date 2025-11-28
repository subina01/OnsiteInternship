using LibraryManagement.Application.Contracts.DTOs.Authors;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace LibraryManagement.Application.Contracts.Services;

public interface IAuthorAppService : ICrudAppService<
    AuthorDto,
    Guid,
    GetAuthorListInput,
    CreateUpdateAuthorDto,
    CreateUpdateAuthorDto>
{
    Task<List<AuthorDto>> GetAuthorsByNationalityAsync(string nationality);
}
