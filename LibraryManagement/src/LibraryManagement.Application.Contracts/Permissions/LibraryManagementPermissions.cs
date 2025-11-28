using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Contracts.Permissions;

public static class LibraryManagementPermissions
{
    public const string GroupName = "LibraryManagement";

    public static class Books
    {
        public const string Default = GroupName + ".Books";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Authors
    {
        public const string Default = GroupName + ".Authors";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Categories
    {
        public const string Default = GroupName + ".Categories";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Publishers
    {
        public const string Default = GroupName + ".Publishers";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Members
    {
        public const string Default = GroupName + ".Members";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Loans
    {
        public const string Default = GroupName + ".Loans";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Return = Default + ".Return";
        public const string Renew = Default + ".Renew";
    }

    public static class Reservations
    {
        public const string Default = GroupName + ".Reservations";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Cancel = Default + ".Cancel";
    }

    public static class Reports
    {
        public const string Default = GroupName + ".Reports";
        public const string View = Default + ".View";
        public const string Export = Default + ".Export";
    }
}
