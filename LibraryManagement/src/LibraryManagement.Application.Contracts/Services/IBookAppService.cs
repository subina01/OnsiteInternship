using LibraryManagement.Application.Contracts.DTOs.Books;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace LibraryManagement.Application.Contracts.Services;

public interface IBookAppService : ICrudAppService<
    BookDto,
    Guid,
    GetBookListInput,
    CreateUpdateBookDto,
    CreateUpdateBookDto>
{
    Task<List<BookDto>> GetAvailableBooksAsync();
    Task<List<BookDto>> GetBooksByAuthorAsync(Guid authorId);
    Task<List<BookDto>> GetBooksByCategoryAsync(Guid categoryId);
    Task<List<BookDto>> GetBooksByPublisherAsync(Guid publisherId);
    Task<BookDto> GetByIsbnAsync(string isbn);
}
