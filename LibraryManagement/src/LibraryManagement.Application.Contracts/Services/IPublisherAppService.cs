using LibraryManagement.Application.Contracts.DTOs.Publishers;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace LibraryManagement.Application.Contracts.Services;

public interface IPublisherAppService : ICrudAppService<
    PublisherDto,
    Guid,
    GetPublisherListInput,
    CreateUpdatePublisherDto,
    CreateUpdatePublisherDto>
{
}
