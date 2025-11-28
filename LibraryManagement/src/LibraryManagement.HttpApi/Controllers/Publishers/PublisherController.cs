using Asp.Versioning;
using LibraryManagement.Application.Contracts.DTOs.Publishers;
using LibraryManagement.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace LibraryManagement.HttpApi.Controllers.Publishers;

[RemoteService(Name = "LibraryManagement")]
[Area("libraryManagement")]
[ControllerName("Publisher")]
[Route("api/library-management/publishers")]
public class PublisherController : AbpController
{
    private readonly IPublisherAppService _publisherAppService;

    public PublisherController(IPublisherAppService publisherAppService)
    {
        _publisherAppService = publisherAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<PublisherDto> GetAsync(Guid id)
    {
        return _publisherAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<PublisherDto>> GetListAsync(GetPublisherListInput input)
    {
        return _publisherAppService.GetListAsync(input);
    }

    [HttpPost]
    public virtual Task<PublisherDto> CreateAsync(CreateUpdatePublisherDto input)
    {
        return _publisherAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<PublisherDto> UpdateAsync(Guid id, CreateUpdatePublisherDto input)
    {
        return _publisherAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _publisherAppService.DeleteAsync(id);
    }
}
