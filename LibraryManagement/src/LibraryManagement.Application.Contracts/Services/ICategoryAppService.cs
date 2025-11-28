using LibraryManagement.Application.Contracts.DTOs.Categories;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace LibraryManagement.Application.Contracts.Services;

public interface ICategoryAppService : ICrudAppService<
    CategoryDto,
    Guid,
    GetCategoryListInput,
    CreateUpdateCategoryDto,
    CreateUpdateCategoryDto>
{
}
