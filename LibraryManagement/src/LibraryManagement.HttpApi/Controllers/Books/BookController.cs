using Asp.Versioning;
using LibraryManagement.Application.Contracts.DTOs.Books;
using LibraryManagement.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace LibraryManagement.HttpApi.Controllers.Books;

[RemoteService(Name = "LibraryManagement")]
[Area("libraryManagement")]
[ControllerName("Book")]
[Route("api/library-management/books")]
public class BookController : AbpController
{
    private readonly IBookAppService _bookAppService;

    public BookController(IBookAppService bookAppService)
    {
        _bookAppService = bookAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<BookDto> GetAsync(Guid id)
    {
        return _bookAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<BookDto>> GetListAsync(GetBookListInput input)
    {
        return _bookAppService.GetListAsync(input);
    }

    [HttpPost]
    public virtual Task<BookDto> CreateAsync(CreateUpdateBookDto input)
    {
        return _bookAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<BookDto> UpdateAsync(Guid id, CreateUpdateBookDto input)
    {
        return _bookAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _bookAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("available")]
    public virtual Task<List<BookDto>> GetAvailableBooksAsync()
    {
        return _bookAppService.GetAvailableBooksAsync();
    }

    [HttpGet]
    [Route("by-author/{authorId}")]
    public virtual Task<List<BookDto>> GetBooksByAuthorAsync(Guid authorId)
    {
        return _bookAppService.GetBooksByAuthorAsync(authorId);
    }

    [HttpGet]
    [Route("by-category/{categoryId}")]
    public virtual Task<List<BookDto>> GetBooksByCategoryAsync(Guid categoryId)
    {
        return _bookAppService.GetBooksByCategoryAsync(categoryId);
    }

    [HttpGet]
    [Route("by-publisher/{publisherId}")]
    public virtual Task<List<BookDto>> GetBooksByPublisherAsync(Guid publisherId)
    {
        return _bookAppService.GetBooksByPublisherAsync(publisherId);
    }

    [HttpGet]
    [Route("by-isbn/{isbn}")]
    public virtual Task<BookDto> GetByIsbnAsync(string isbn)
    {
        return _bookAppService.GetByIsbnAsync(isbn);
    }
}
