using LibraryManagement.Domain.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace LibraryManagement.Application.EventHandlers;

public class LoanReturnedEventHandler : ILocalEventHandler<LoanReturnedEvent>, ITransientDependency
{
    private readonly ILogger<LoanReturnedEventHandler> _logger;

    public LoanReturnedEventHandler(ILogger<LoanReturnedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleEventAsync(LoanReturnedEvent eventData)
    {
        _logger.LogInformation(
            "Loan returned: LoanId={LoanId}, ReturnDate={ReturnDate}, Fine={Fine}, WasOverdue={WasOverdue}",
            eventData.LoanId,
            eventData.ReturnDate,
            eventData.Fine,
            eventData.WasOverdue);

        if (eventData.WasOverdue)
        {
            _logger.LogWarning(
                "Overdue loan returned: LoanId={LoanId}, Fine={Fine}",
                eventData.LoanId,
                eventData.Fine);

            // TODO: Send fine notification email
            // TODO: Process fine payment
        }

        // TODO: Update book availability
        // TODO: Notify waiting reservations
        // TODO: Update statistics

        return Task.CompletedTask;
    }
}
