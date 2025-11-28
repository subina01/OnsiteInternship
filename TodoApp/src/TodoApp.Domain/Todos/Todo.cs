using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;

namespace TodoApp.Todos;

public class Todo : AuditedAggregateRoot<Guid>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }

    // Private constructor for EF Core
    private Todo()
    {
    }

    // Public constructor for creating new todos
    public Todo(Guid id, string title, string description = null, DateTime? dueDate = null)
        : base(id)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        IsCompleted = false;
    }

    public void Complete()
    {
        IsCompleted = true;
    }

    public void Reopen()
    {
        IsCompleted = false;
    }

    public void UpdateTitle(string title)
    {
        Title = title;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
    }

    public void UpdateDueDate(DateTime? dueDate)
    {
        DueDate = dueDate;
    }
}