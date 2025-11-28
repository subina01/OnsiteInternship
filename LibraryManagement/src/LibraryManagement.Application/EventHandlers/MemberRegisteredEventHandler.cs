using LibraryManagement.Domain.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace LibraryManagement.Application.EventHandlers;

public class MemberRegisteredEventHandler : ILocalEventHandler<MemberRegisteredEvent>, ITransientDependency
{
    private readonly ILogger<MemberRegisteredEventHandler> _logger;

    public MemberRegisteredEventHandler(ILogger<MemberRegisteredEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleEventAsync(MemberRegisteredEvent eventData)
    {
        _logger.LogInformation(
            "New member registered: MemberId={MemberId}, FullName={FullName}, Email={Email}, MembershipNumber={MembershipNumber}",
            eventData.MemberId,
            eventData.FullName,
            eventData.Email,
            eventData.MembershipNumber);

        // TODO: Send welcome email
        // TODO: Generate membership card
        // TODO: Add to mailing list
        // TODO: Update statistics

        return Task.CompletedTask;
    }
}