using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;
using static LibraryManagement.Domain.Shared.Constants.LibraryManagementConsts;

namespace LibraryManagement.Domain.Entities;

/// <summary>
/// BookAuthor - Junction entity for Book-Author many-to-many relationship
/// </summary>
public class BookAuthor : Entity
{
    public Guid BookId { get; private set; }
    public Guid AuthorId { get; private set; }

    // Navigation properties
    public virtual Book Book { get; private set; } = null!;
    public virtual Author Author { get; private set; } = null!;

    private BookAuthor()
    {
    }

    internal BookAuthor(Guid bookId, Guid authorId)
    {
        BookId = bookId;
        AuthorId = authorId;
    }

    public override object[] GetKeys()
    {
        return new object[] { BookId, AuthorId };
    }
}