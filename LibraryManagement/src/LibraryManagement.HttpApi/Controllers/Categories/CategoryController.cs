using Asp.Versioning;
using LibraryManagement.Application.Contracts.DTOs.Categories;
using LibraryManagement.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace LibraryManagement.HttpApi.Controllers.Categories;

[RemoteService(Name = "LibraryManagement")]
[Area("libraryManagement")]
[ControllerName("Category")]
[Route("api/library-management/categories")]
public class CategoryController : AbpController
{
    private readonly ICategoryAppService _categoryAppService;

    public CategoryController(ICategoryAppService categoryAppService)
    {
        _categoryAppService = categoryAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<CategoryDto> GetAsync(Guid id)
    {
        return _categoryAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<CategoryDto>> GetListAsync(GetCategoryListInput input)
    {
        return _categoryAppService.GetListAsync(input);
    }

    [HttpPost]
    public virtual Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto input)
    {
        return _categoryAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<CategoryDto> UpdateAsync(Guid id, CreateUpdateCategoryDto input)
    {
        return _categoryAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _categoryAppService.DeleteAsync(id);
    }
}
