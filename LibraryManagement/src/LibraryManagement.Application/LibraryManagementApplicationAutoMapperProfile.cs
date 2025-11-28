using AutoMapper;
using LibraryManagement.Application.Contracts.DTOs.Authors;
using LibraryManagement.Application.Contracts.DTOs.Books;
using LibraryManagement.Application.Contracts.DTOs.Categories;
using LibraryManagement.Application.Contracts.DTOs.Loans;
using LibraryManagement.Application.Contracts.DTOs.Members;
using LibraryManagement.Application.Contracts.DTOs.Publishers;
using LibraryManagement.Application.Contracts.DTOs.Reservations;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application;

public class LibraryManagementApplicationAutoMapperProfile : Profile
{
    public LibraryManagementApplicationAutoMapperProfile()
    {
        // Book mappings
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.ISBN, opt => opt.MapFrom(src => src.ISBN.Value))
            .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher != null ? src.Publisher.Name : null))
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.BookAuthors.Select(ba => ba.Author)))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.BookCategories.Select(bc => bc.Category)));

        CreateMap<CreateUpdateBookDto, Book>()
            .ForMember(dest => dest.ISBN, opt => opt.Ignore())
            .ForMember(dest => dest.BookAuthors, opt => opt.Ignore())
            .ForMember(dest => dest.BookCategories, opt => opt.Ignore());

        // Author mappings
        CreateMap<Author, AuthorDto>()
            .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.BookAuthors.Count));

        CreateMap<Author, AuthorBasicDto>();

        CreateMap<CreateUpdateAuthorDto, Author>()
            .ForMember(dest => dest.BookAuthors, opt => opt.Ignore());

        // Category mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.BookCategories.Count));

        CreateMap<Category, CategoryBasicDto>();

        CreateMap<CreateUpdateCategoryDto, Category>()
            .ForMember(dest => dest.BookCategories, opt => opt.Ignore());

        // Publisher mappings
        CreateMap<Publisher, PublisherDto>()
            .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books.Count));

        CreateMap<CreateUpdatePublisherDto, Publisher>()
            .ForMember(dest => dest.Books, opt => opt.Ignore());

        // Member mappings
        CreateMap<Member, MemberDto>()
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired()))
            .ForMember(dest => dest.ActiveLoanCount, opt => opt.Ignore());

        CreateMap<Address, AddressDto>().ReverseMap();

        CreateMap<CreateUpdateMemberDto, Member>()
            .ForMember(dest => dest.Address, opt => opt.Ignore());

        CreateMap<CreateUpdateAddressDto, Address>()
            .ConvertUsing(src => Address.Create(src.Street, src.City, src.State, src.ZipCode, src.Country));

        // Loan mappings
        CreateMap<Loan, LoanDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.BookISBN, opt => opt.MapFrom(src => src.Book.ISBN.Value))
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.FullName))
            .ForMember(dest => dest.MembershipNumber, opt => opt.MapFrom(src => src.Member.MembershipNumber))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue()))
            .ForMember(dest => dest.OverdueDays, opt => opt.MapFrom(src => src.GetOverdueDays()));

        // Reservation mappings
        CreateMap<Reservation, ReservationDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.BookISBN, opt => opt.MapFrom(src => src.Book.ISBN.Value))
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.FullName))
            .ForMember(dest => dest.MembershipNumber, opt => opt.MapFrom(src => src.Member.MembershipNumber))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired()));
    }
}
