using LibraryManagement.Domain.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace LibraryManagement.Application.EventHandlers;

public class LoanCreatedEventHandler : ILocalEventHandler<LoanCreatedEvent>, ITransientDependency
{
    private readonly ILogger<LoanCreatedEventHandler> _logger;

    public LoanCreatedEventHandler(ILogger<LoanCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleEventAsync(LoanCreatedEvent eventData)
    {
        _logger.LogInformation(
            "Loan created: LoanId={LoanId}, BookId={BookId}, MemberId={MemberId}, DueDate={DueDate}",
            eventData.LoanId,
            eventData.BookId,
            eventData.MemberId,
            eventData.DueDate);

        // TODO: Send email notification to member
        // TODO: Update statistics
        // TODO: Trigger other business processes

        return Task.CompletedTask;
    }
}
