using AutoMapper.Internal.Mappers;
using LibraryManagement.Application.Contracts.DTOs.Authors;
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

namespace LibraryManagement.Application.Services.Authors;

public class AuthorAppService : CrudAppService<
    Author,
    AuthorDto,
    Guid,
    GetAuthorListInput,
    CreateUpdateAuthorDto,
    CreateUpdateAuthorDto>, IAuthorAppService
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorAppService(IAuthorRepository repository)
        : base(repository)
    {
        _authorRepository = repository;
    }

    protected override async Task<IQueryable<Author>> CreateFilteredQueryAsync(GetAuthorListInput input)
    {
        var query = await base.CreateFilteredQueryAsync(input);

        query = query
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                x => x.Name.Contains(input.Filter!))
            .WhereIf(!input.Nationality.IsNullOrWhiteSpace(),
                x => x.Nationality == input.Nationality);

        return query;
    }

    public override async Task<AuthorDto> CreateAsync(CreateUpdateAuthorDto input)
    {
        var author = new Author(
            GuidGenerator.Create(),
            input.Name,
            input.Biography,
            input.BirthDate,
            input.Nationality);

        await _authorRepository.InsertAsync(author);

        return await MapToGetOutputDtoAsync(author);
    }

    public override async Task<AuthorDto> UpdateAsync(Guid id, CreateUpdateAuthorDto input)
    {
        var author = await _authorRepository.GetAsync(id);

        author.ChangeName(input.Name)
              .SetBiography(input.Biography)
              .SetBirthDate(input.BirthDate)
              .SetNationality(input.Nationality);

        await _authorRepository.UpdateAsync(author);

        return await MapToGetOutputDtoAsync(author);
    }

    public override async Task DeleteAsync(Guid id)
    {
        if (await _authorRepository.HasBooksAsync(id))
        {
            throw new Volo.Abp.BusinessException(LibraryManagementErrorCodes.AuthorHasBooks)
                .WithData("AuthorId", id);
        }

        await base.DeleteAsync(id);
    }

    public async Task<List<AuthorDto>> GetAuthorsByNationalityAsync(string nationality)
    {
        var authors = await _authorRepository.GetListByNationalityAsync(nationality);
        return ObjectMapper.Map<List<Author>, List<AuthorDto>>(authors);
    }
}
