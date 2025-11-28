using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Events;

/// <summary>
/// Domain Event - Raised when a new book is created
/// </summary>
[Serializable]
public class BookCreatedEvent
{
    public Guid BookId { get; }
    public string Title { get; }
    public string ISBN { get; }
    public int Quantity { get; }

    public BookCreatedEvent(Book book)
    {
        BookId = book.Id;
        Title = book.Title;
        ISBN = book.ISBN.Value;
        Quantity = book.Quantity;
    }
}
