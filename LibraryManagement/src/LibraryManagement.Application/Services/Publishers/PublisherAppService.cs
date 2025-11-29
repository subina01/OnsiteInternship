using LibraryManagement.Application.Contracts.DTOs.Publishers;
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

namespace LibraryManagement.Application.Services.Publishers;

public class PublisherAppService : CrudAppService<
    Publisher,
    PublisherDto,
    Guid,
    GetPublisherListInput,
    CreateUpdatePublisherDto,
    CreateUpdatePublisherDto>, IPublisherAppService
{
    private readonly IPublisherRepository _publisherRepository;

    public PublisherAppService(IPublisherRepository repository)
        : base(repository)
    {
        _publisherRepository = repository;
    }

    protected override async Task<IQueryable<Publisher>> CreateFilteredQueryAsync(GetPublisherListInput input)
    {
        var query = await base.CreateFilteredQueryAsync(input);

        query = query
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                x => x.Name.Contains(input.Filter!));

        return query;
    }

    public override async Task<PublisherDto> CreateAsync(CreateUpdatePublisherDto input)
    {
        var publisher = new Publisher(
            GuidGenerator.Create(),
            input.Name,
            input.Website,
            input.ContactEmail,
            input.ContactPhone);

        await _publisherRepository.InsertAsync(publisher);

        return await MapToGetOutputDtoAsync(publisher);
    }

    public override async Task<PublisherDto> UpdateAsync(Guid id, CreateUpdatePublisherDto input)
    {
        var publisher = await _publisherRepository.GetAsync(id);

        publisher.ChangeName(input.Name)
                 .SetWebsite(input.Website)
                 .SetContactEmail(input.ContactEmail)
                 .SetContactPhone(input.ContactPhone);

        await _publisherRepository.UpdateAsync(publisher);

        return await MapToGetOutputDtoAsync(publisher);
    }

    public override async Task DeleteAsync(Guid id)
    {
        if (await _publisherRepository.HasBooksAsync(id))
        {
            throw new Volo.Abp.BusinessException(LibraryManagementErrorCodes.PublisherHasBooks)
                .WithData("PublisherId", id);
        }

        await base.DeleteAsync(id);
    }
}
