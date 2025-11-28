using LibraryManagement.Domain.Shared.Localization.LibraryManagement;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace LibraryManagement.Application.Contracts.Permissions;

public class LibraryManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var libraryManagementGroup = context.AddGroup(
            LibraryManagementPermissions.GroupName,
            L("Permission:LibraryManagement"));

        // Books Permissions
        var booksPermission = libraryManagementGroup.AddPermission(
            LibraryManagementPermissions.Books.Default,
            L("Permission:Books"));
        booksPermission.AddChild(
            LibraryManagementPermissions.Books.Create,
            L("Permission:Books.Create"));
        booksPermission.AddChild(
            LibraryManagementPermissions.Books.Edit,
            L("Permission:Books.Edit"));
        booksPermission.AddChild(
            LibraryManagementPermissions.Books.Delete,
            L("Permission:Books.Delete"));

        // Authors Permissions
        var authorsPermission = libraryManagementGroup.AddPermission(
            LibraryManagementPermissions.Authors.Default,
            L("Permission:Authors"));
        authorsPermission.AddChild(
            LibraryManagementPermissions.Authors.Create,
            L("Permission:Authors.Create"));
        authorsPermission.AddChild(
            LibraryManagementPermissions.Authors.Edit,
            L("Permission:Authors.Edit"));
        authorsPermission.AddChild(
            LibraryManagementPermissions.Authors.Delete,
            L("Permission:Authors.Delete"));

        // Categories Permissions
        var categoriesPermission = libraryManagementGroup.AddPermission(
            LibraryManagementPermissions.Categories.Default,
            L("Permission:Categories"));
        categoriesPermission.AddChild(
            LibraryManagementPermissions.Categories.Create,
            L("Permission:Categories.Create"));
        categoriesPermission.AddChild(
            LibraryManagementPermissions.Categories.Edit,
            L("Permission:Categories.Edit"));
        categoriesPermission.AddChild(
            LibraryManagementPermissions.Categories.Delete,
            L("Permission:Categories.Delete"));

        // Publishers Permissions
        var publishersPermission = libraryManagementGroup.AddPermission(
            LibraryManagementPermissions.Publishers.Default,
            L("Permission:Publishers"));
        publishersPermission.AddChild(
            LibraryManagementPermissions.Publishers.Create,
            L("Permission:Publishers.Create"));
        publishersPermission.AddChild(
            LibraryManagementPermissions.Publishers.Edit,
            L("Permission:Publishers.Edit"));
        publishersPermission.AddChild(
            LibraryManagementPermissions.Publishers.Delete,
            L("Permission:Publishers.Delete"));

        // Members Permissions
        var membersPermission = libraryManagementGroup.AddPermission(
            LibraryManagementPermissions.Members.Default,
            L("Permission:Members"));
        membersPermission.AddChild(
            LibraryManagementPermissions.Members.Create,
            L("Permission:Members.Create"));
        membersPermission.AddChild(
            LibraryManagementPermissions.Members.Edit,
            L("Permission:Members.Edit"));
        membersPermission.AddChild(
            LibraryManagementPermissions.Members.Delete,
            L("Permission:Members.Delete"));

        // Loans Permissions
        var loansPermission = libraryManagementGroup.AddPermission(
            LibraryManagementPermissions.Loans.Default,
            L("Permission:Loans"));
        loansPermission.AddChild(
            LibraryManagementPermissions.Loans.Create,
            L("Permission:Loans.Create"));
        loansPermission.AddChild(
            LibraryManagementPermissions.Loans.Edit,
            L("Permission:Loans.Edit"));
        loansPermission.AddChild(
            LibraryManagementPermissions.Loans.Delete,
            L("Permission:Loans.Delete"));
        loansPermission.AddChild(
            LibraryManagementPermissions.Loans.Return,
            L("Permission:Loans.Return"));
        loansPermission.AddChild(
            LibraryManagementPermissions.Loans.Renew,
            L("Permission:Loans.Renew"));

        // Reservations Permissions
        var reservationsPermission = libraryManagementGroup.AddPermission(
            LibraryManagementPermissions.Reservations.Default,
            L("Permission:Reservations"));
        reservationsPermission.AddChild(
            LibraryManagementPermissions.Reservations.Create,
            L("Permission:Reservations.Create"));
        reservationsPermission.AddChild(
            LibraryManagementPermissions.Reservations.Edit,
            L("Permission:Reservations.Edit"));
        reservationsPermission.AddChild(
            LibraryManagementPermissions.Reservations.Delete,
            L("Permission:Reservations.Delete"));
        reservationsPermission.AddChild(
            LibraryManagementPermissions.Reservations.Cancel,
            L("Permission:Reservations.Cancel"));

        // Reports Permissions
        var reportsPermission = libraryManagementGroup.AddPermission(
            LibraryManagementPermissions.Reports.Default,
            L("Permission:Reports"));
        reportsPermission.AddChild(
            LibraryManagementPermissions.Reports.View,
            L("Permission:Reports.View"));
        reportsPermission.AddChild(
            LibraryManagementPermissions.Reports.Export,
            L("Permission:Reports.Export"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<LibraryManagementResource>(name);
    }
}
