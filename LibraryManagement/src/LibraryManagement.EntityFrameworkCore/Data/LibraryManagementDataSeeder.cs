using LibraryManagement.Domain.DomainServices;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace LibraryManagement.EntityFrameworkCore.Data;

public class LibraryManagementDataSeeder : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Author, Guid> _authorRepository;
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly IRepository<Publisher, Guid> _publisherRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly BookManager _bookManager;
    private readonly MemberManager _memberManager;

    public LibraryManagementDataSeeder(
        IRepository<Author, Guid> authorRepository,
        IRepository<Category, Guid> categoryRepository,
        IRepository<Publisher, Guid> publisherRepository,
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        BookManager bookManager,
        MemberManager memberManager)
    {
        _authorRepository = authorRepository;
        _categoryRepository = categoryRepository;
        _publisherRepository = publisherRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _bookManager = bookManager;
        _memberManager = memberManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // Seed only if database is empty
        if (await _bookRepository.GetCountAsync() > 0)
        {
            return;
        }

        // Seed test user for authentication
        await SeedTestUserAsync();

        // Seed Categories
        var fictionCategory = await SeedCategoryAsync("Fiction", "Fictional literature including novels and short stories");
        var scienceCategory = await SeedCategoryAsync("Science", "Scientific books and research materials");
        var historyCategory = await SeedCategoryAsync("History", "Historical books and documentaries");
        var technologyCategory = await SeedCategoryAsync("Technology", "Technology and computer science books");

        // Seed Authors
        var author1 = await SeedAuthorAsync("J.K. Rowling", "British author best known for Harry Potter series", new DateTime(1965, 7, 31), "British");
        var author2 = await SeedAuthorAsync("George Orwell", "English novelist and essayist", new DateTime(1903, 6, 25), "British");
        var author3 = await SeedAuthorAsync("Isaac Asimov", "American science fiction writer", new DateTime(1920, 1, 2), "American");

        // Seed Publishers
        var publisher1 = await SeedPublisherAsync("Bloomsbury Publishing", "https://www.bloomsbury.com", "info@bloomsbury.com", "+44-20-7631-5600");
        var publisher2 = await SeedPublisherAsync("Penguin Books", "https://www.penguin.com", "info@penguin.com", "+44-20-7010-3000");
        var publisher3 = await SeedPublisherAsync("Doubleday", "https://www.penguinrandomhouse.com/publishers/doubleday", "info@doubleday.com", "+1-212-751-2600");

        // Seed Books
        var book1 = await SeedBookAsync(
            "Harry Potter and the Philosopher's Stone",
            "9780747532699",
            1997,
            1,
            5,
            publisher1.Id,
            "The first novel in the Harry Potter series",
            "English",
            223);
        book1.AddAuthor(author1.Id);
        book1.AddCategory(fictionCategory.Id);
        await _bookRepository.UpdateAsync(book1);

        var book2 = await SeedBookAsync(
            "1984",
            "9780451524935",
            1949,
            1,
            10,
            publisher2.Id,
            "A dystopian social science fiction novel",
            "English",
            328);
        book2.AddAuthor(author2.Id);
        book2.AddCategory(fictionCategory.Id);
        await _bookRepository.UpdateAsync(book2);

        var book3 = await SeedBookAsync(
            "Foundation",
            "9780553293357",
            1951,
            1,
            8,
            publisher3.Id,
            "A science fiction novel by Isaac Asimov",
            "English",
            255);
        book3.AddAuthor(author3.Id);
        book3.AddCategory(scienceCategory.Id);
        book3.AddCategory(fictionCategory.Id);
        await _bookRepository.UpdateAsync(book3);

        // Seed Members
        await SeedMemberAsync(
            "John",
            "Doe",
            "MEM001",
            "john.doe@example.com",
            MembershipType.Standard,
            "+1-555-0101");

        await SeedMemberAsync(
            "Jane",
            "Smith",
            "MEM002",
            "jane.smith@example.com",
            MembershipType.Premium,
            "+1-555-0102");

        await SeedMemberAsync(
            "Bob",
            "Johnson",
            "MEM003",
            "bob.johnson@example.com",
            MembershipType.Student,
            "+1-555-0103");
    }

    private async Task<Category> SeedCategoryAsync(string name, string? description = null)
    {
        // Create a new Category instance using a public factory method or property setters if available
        var category = (Category)Activator.CreateInstance(typeof(Category), true)!;
        category.ChangeName(name);
        category.SetDescription(description);
        typeof(Category).GetProperty("Id")!.SetValue(category, Guid.NewGuid());
        return await _categoryRepository.InsertAsync(category);
    }

    private async Task<Author> SeedAuthorAsync(
        string name,
        string? biography = null,
        DateTime? birthDate = null,
        string? nationality = null)
    {
        // Use Activator to create an instance since the constructor is not accessible
        var author = (Author)Activator.CreateInstance(typeof(Author), true)!;
        author.ChangeName(name);
        author.SetBiography(biography);
        author.SetBirthDate(birthDate);
        author.SetNationality(nationality);
        typeof(Author).GetProperty("Id")!.SetValue(author, Guid.NewGuid());
        return await _authorRepository.InsertAsync(author);
    }

    // Replace the body of SeedPublisherAsync to use reflection for instantiating Publisher
    private async Task<Publisher> SeedPublisherAsync(
        string name,
        string? website = null,
        string? contactEmail = null,
        string? contactPhone = null)
    {
        // Use reflection to create an instance since the constructor is not accessible
        var publisher = (Publisher)Activator.CreateInstance(typeof(Publisher), true)!;
        typeof(Publisher).GetProperty("Id")!.SetValue(publisher, Guid.NewGuid());
        publisher.ChangeName(name);
        publisher.SetWebsite(website);
        publisher.SetContactEmail(contactEmail);
        publisher.SetContactPhone(contactPhone);
        return await _publisherRepository.InsertAsync(publisher);
    }

    private async Task<Book> SeedBookAsync(
        string title,
        string isbn,
        int publicationYear,
        int edition,
        int quantity,
        Guid? publisherId = null,
        string? description = null,
        string? language = null,
        int? pageCount = null)
    {
        var book = await _bookManager.CreateAsync(title, isbn, publicationYear, edition, quantity, publisherId);

        if (description != null)
            book.SetDescription(description);

        if (language != null)
            book.SetLanguage(language);

        if (pageCount.HasValue)
            book.SetPageCount(pageCount.Value);

        return await _bookRepository.InsertAsync(book);
    }

    private async Task<Member> SeedMemberAsync(
        string firstName,
        string lastName,
        string membershipNumber,
        string email,
        MembershipType membershipType,
        string? phoneNumber = null)
    {
        var member = await _memberManager.CreateAsync(
            firstName,
            lastName,
            membershipNumber,
            email,
            membershipType,
            phoneNumber);

        return         await _memberRepository.InsertAsync(member);
    }

    private async Task SeedTestUserAsync()
    {
        // This is a simplified version - in a real application you'd use proper user management
        // For testing purposes, we'll assume authentication is handled elsewhere
    }
}
