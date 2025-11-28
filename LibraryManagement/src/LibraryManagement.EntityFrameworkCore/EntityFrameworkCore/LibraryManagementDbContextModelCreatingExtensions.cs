using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace LibraryManagement.EntityFrameworkCore.EntityFrameworkCore;

public static class LibraryManagementDbContextModelCreatingExtensions
{
    public static void ConfigureLibraryManagement(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure your own tables/entities inside here */

        // Books Module
        builder.Entity<Book>(b =>
        {
            b.ToTable(LibraryManagementDbProperties.DbTablePrefix + "Books",
                LibraryManagementDbProperties.DbSchema);

            b.ConfigureByConvention(); // Auto-configure audit properties

            b.Property(x => x.Title).IsRequired().HasMaxLength(LibraryManagementConsts.MaxTitleLength);
            b.Property(x => x.Edition).IsRequired();
            b.Property(x => x.PublicationYear).IsRequired();
            b.Property(x => x.Quantity).IsRequired();
            b.Property(x => x.AvailableQuantity).IsRequired();
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.Description).HasMaxLength(LibraryManagementConsts.MaxDescriptionLength);
            b.Property(x => x.Language).HasMaxLength(LibraryManagementConsts.MaxShortTextLength);
            b.Property(x => x.PageCount);

            // Value Object - ISBN (Owned Entity approach)
            b.OwnsOne(x => x.ISBN, isbn =>
            {
                isbn.Property(p => p.Value)
                    .IsRequired()
                    .HasMaxLength(LibraryManagementConsts.MaxIsbnLength)
                    .HasColumnName("ISBN");
            });

            // Indexes
            b.HasIndex(x => x.PublisherId);
            b.HasIndex(x => x.Status);

            // Relationships
            b.HasOne(x => x.Publisher)
                .WithMany(x => x.Books)
                .HasForeignKey(x => x.PublisherId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasMany(x => x.BookAuthors)
                .WithOne(x => x.Book)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.BookCategories)
                .WithOne(x => x.Book)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Author>(b =>
        {
            b.ToTable(LibraryManagementDbProperties.DbTablePrefix + "Authors",
                LibraryManagementDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(LibraryManagementConsts.MaxNameLength);
            b.Property(x => x.Biography).HasMaxLength(LibraryManagementConsts.Authors.MaxBiographyLength);
            b.Property(x => x.BirthDate);
            b.Property(x => x.Nationality).HasMaxLength(LibraryManagementConsts.MaxShortTextLength);

            // Indexes
            b.HasIndex(x => x.Name);

            b.HasMany(x => x.BookAuthors)
                .WithOne(x => x.Author)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Category>(b =>
        {
            b.ToTable(LibraryManagementDbProperties.DbTablePrefix + "Categories",
                LibraryManagementDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(LibraryManagementConsts.MaxNameLength);
            b.Property(x => x.Description).HasMaxLength(LibraryManagementConsts.MaxDescriptionLength);

            // Indexes
            b.HasIndex(x => x.Name);

            b.HasMany(x => x.BookCategories)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Publisher>(b =>
        {
            b.ToTable(LibraryManagementDbProperties.DbTablePrefix + "Publishers",
                LibraryManagementDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(LibraryManagementConsts.MaxNameLength);
            b.Property(x => x.Website).HasMaxLength(LibraryManagementConsts.MaxUrlLength);
            b.Property(x => x.ContactEmail).HasMaxLength(LibraryManagementConsts.MaxEmailLength);
            b.Property(x => x.ContactPhone).HasMaxLength(LibraryManagementConsts.MaxPhoneNumberLength);

            // Indexes
            b.HasIndex(x => x.Name);
        });

        builder.Entity<BookAuthor>(b =>
        {
            b.ToTable(LibraryManagementDbProperties.DbTablePrefix + "BookAuthors",
                LibraryManagementDbProperties.DbSchema);

            b.HasKey(x => new { x.BookId, x.AuthorId });

            // Indexes for performance
            b.HasIndex(x => x.BookId);
            b.HasIndex(x => x.AuthorId);
        });

        builder.Entity<BookCategory>(b =>
        {
            b.ToTable(LibraryManagementDbProperties.DbTablePrefix + "BookCategories",
                LibraryManagementDbProperties.DbSchema);

            b.HasKey(x => new { x.BookId, x.CategoryId });

            // Indexes for performance
            b.HasIndex(x => x.BookId);
            b.HasIndex(x => x.CategoryId);
        });

        // Members Module
        builder.Entity<Member>(b =>
        {
            b.ToTable(LibraryManagementDbProperties.DbTablePrefix + "Members",
                LibraryManagementDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.FirstName).IsRequired().HasMaxLength(LibraryManagementConsts.MaxNameLength);
            b.Property(x => x.LastName).IsRequired().HasMaxLength(LibraryManagementConsts.MaxNameLength);
            b.Property(x => x.MembershipNumber).IsRequired()
                .HasMaxLength(LibraryManagementConsts.Members.MaxMembershipNumberLength);
            b.Property(x => x.Email).IsRequired().HasMaxLength(LibraryManagementConsts.MaxEmailLength);
            b.Property(x => x.PhoneNumber).HasMaxLength(LibraryManagementConsts.MaxPhoneNumberLength);
            b.Property(x => x.MembershipType).IsRequired();
            b.Property(x => x.JoinDate).IsRequired();
            b.Property(x => x.ExpiryDate).IsRequired();
            b.Property(x => x.MaxLoanLimit).IsRequired();

            // Value Object - Address (Owned Entity)
            b.OwnsOne(x => x.Address, address =>
            {
                address.Property(p => p.Street)
                    .HasMaxLength(LibraryManagementConsts.Address.MaxStreetLength)
                    .HasColumnName("AddressStreet");
                address.Property(p => p.City)
                    .HasMaxLength(LibraryManagementConsts.Address.MaxCityLength)
                    .HasColumnName("AddressCity");
                address.Property(p => p.State)
                    .HasMaxLength(LibraryManagementConsts.Address.MaxStateLength)
                    .HasColumnName("AddressState");
                address.Property(p => p.ZipCode)
                    .HasMaxLength(LibraryManagementConsts.Address.MaxZipCodeLength)
                    .HasColumnName("AddressZipCode");
                address.Property(p => p.Country)
                    .HasMaxLength(LibraryManagementConsts.Address.MaxCountryLength)
                    .HasColumnName("AddressCountry");
            });

            // Indexes
            b.HasIndex(x => x.MembershipNumber).IsUnique();
            b.HasIndex(x => x.Email).IsUnique();
            b.HasIndex(x => x.MembershipType);
            b.HasIndex(x => x.ExpiryDate);
        });

        // Loans Module
        builder.Entity<Loan>(b =>
        {
            b.ToTable(LibraryManagementDbProperties.DbTablePrefix + "Loans",
                LibraryManagementDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.BookId).IsRequired();
            b.Property(x => x.MemberId).IsRequired();
            b.Property(x => x.LoanDate).IsRequired();
            b.Property(x => x.DueDate).IsRequired();
            b.Property(x => x.ReturnDate);
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.RenewalCount).IsRequired().HasDefaultValue(0);
            b.Property(x => x.Fine).IsRequired().HasDefaultValue(0).HasColumnType("decimal(18,2)");
            b.Property(x => x.Notes).HasMaxLength(LibraryManagementConsts.MaxDescriptionLength);

            // Indexes
            b.HasIndex(x => x.BookId);
            b.HasIndex(x => x.MemberId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.DueDate);
            b.HasIndex(x => new { x.Status, x.DueDate }); // Composite index for overdue queries

            // Relationships
            b.HasOne(x => x.Book)
                .WithMany()
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Member)
                .WithMany()
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Reservations Module
        builder.Entity<Reservation>(b =>
        {
            b.ToTable(LibraryManagementDbProperties.DbTablePrefix + "Reservations",
                LibraryManagementDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.BookId).IsRequired();
            b.Property(x => x.MemberId).IsRequired();
            b.Property(x => x.ReservationDate).IsRequired();
            b.Property(x => x.ExpirationDate).IsRequired();
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.ReadyForPickupDate);
            b.Property(x => x.FulfilledDate);
            b.Property(x => x.Notes).HasMaxLength(LibraryManagementConsts.MaxDescriptionLength);

            // Indexes
            b.HasIndex(x => x.BookId);
            b.HasIndex(x => x.MemberId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.ExpirationDate);
            b.HasIndex(x => new { x.Status, x.ExpirationDate }); // Composite for expired reservations

            // Relationships
            b.HasOne(x => x.Book)
                .WithMany()
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Member)
                .WithMany()
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
