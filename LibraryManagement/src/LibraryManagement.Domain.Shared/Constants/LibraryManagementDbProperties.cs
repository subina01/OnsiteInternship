using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Shared.Constants;

/// <summary>
/// Database properties for LibraryManagement module
/// </summary>
public static class LibraryManagementDbProperties
{
    /// <summary>
    /// Default connection string name
    /// This is the KEY used to lookup the connection string from appsettings.json
    /// Change this if you want to use a different connection string name
    /// </summary>
    public static string ConnectionStringName { get; set; } = "Default";

    /// <summary>
    /// Database table prefix for all tables
    /// </summary>
    public static string DbTablePrefix { get; set; } = LibraryManagementConsts.DbTablePrefix.Default;

    /// <summary>
    /// Database schema name
    /// </summary>
    public static string? DbSchema { get; set; } = LibraryManagementConsts.DbSchema.Default;
}
