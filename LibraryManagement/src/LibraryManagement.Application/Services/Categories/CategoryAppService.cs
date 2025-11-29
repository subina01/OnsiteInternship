using LibraryManagement.Application.Contracts.DTOs.Categories;
using LibraryManagement.Application.Contracts.Permissions;
using LibraryManagement.Application.Contracts.Services;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using LibraryManagement.Domain.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;
using Volo.Abp.Guids;

namespace LibraryManagement.Application.Services.Categories;

public class CategoryAppService : CrudAppService<
    Category,
    CategoryDto,
    Guid,
    GetCategoryListInput,
    CreateUpdateCategoryDto,
    CreateUpdateCategoryDto>, ICategoryAppService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryAppService(ICategoryRepository repository)
        : base(repository)
    {
        _categoryRepository = repository;
    }

    protected override async Task<IQueryable<Category>> CreateFilteredQueryAsync(GetCategoryListInput input)
    {
        var query = await base.CreateFilteredQueryAsync(input);

        query = query
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                x => x.Name.Contains(input.Filter!));

        return query;
    }

    public override async Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto input)
    {
        var category = new Category(
            GuidGenerator.Create(),
            input.Name,
            input.Description);

        await _categoryRepository.InsertAsync(category);

        return await MapToGetOutputDtoAsync(category);
    }

    public override async Task<CategoryDto> UpdateAsync(Guid id, CreateUpdateCategoryDto input)
    {
        var category = await _categoryRepository.GetAsync(id);

        category.ChangeName(input.Name)
                .SetDescription(input.Description);

        await _categoryRepository.UpdateAsync(category);

        return await MapToGetOutputDtoAsync(category);
    }

    public override async Task DeleteAsync(Guid id)
    {
        if (await _categoryRepository.HasBooksAsync(id))
        {
            throw new Volo.Abp.BusinessException(LibraryManagementErrorCodes.CategoryHasBooks)
                .WithData("CategoryId", id);
        }

        await base.DeleteAsync(id);
    }
}
