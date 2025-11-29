using Asp.Versioning;
using LibraryManagement.Application.Contracts.DTOs.Authors;
using LibraryManagement.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace LibraryManagement.HttpApi.Controllers.Authors;

[RemoteService(Name = "LibraryManagement")]
[Area("libraryManagement")]
[ControllerName("Author")]
[Route("api/library-management/authors")]
[IgnoreAntiforgeryToken]
public class AuthorController : AbpController
{
    private readonly IAuthorAppService _authorAppService;

    public AuthorController(IAuthorAppService authorAppService)
    {
        _authorAppService = authorAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<AuthorDto> GetAsync(Guid id)
    {
        return _authorAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<AuthorDto>> GetListAsync(GetAuthorListInput input)
    {
        return _authorAppService.GetListAsync(input);
    }

    [HttpPost]
    public virtual Task<AuthorDto> CreateAsync(CreateUpdateAuthorDto input)
    {
        return _authorAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<AuthorDto> UpdateAsync(Guid id, CreateUpdateAuthorDto input)
    {
        return _authorAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _authorAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("by-nationality/{nationality}")]
    public virtual Task<List<AuthorDto>> GetAuthorsByNationalityAsync(string nationality)
    {
        return _authorAppService.GetAuthorsByNationalityAsync(nationality);
    }
}
