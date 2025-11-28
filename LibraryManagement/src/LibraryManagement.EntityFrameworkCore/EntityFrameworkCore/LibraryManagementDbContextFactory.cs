using LibraryManagement.Domain.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.EntityFrameworkCore.EntityFrameworkCore;

/// <summary>
/// This factory is used for design-time tools (like EF Core migrations)
/// The appsettings.json file is optional for this class library.
/// If not found, it will use a hardcoded connection string.
/// In production, use the DbMigrator project or HttpApi.Host project to run migrations.
/// </summary>
public class LibraryManagementDbContextFactory : IDesignTimeDbContextFactory<LibraryManagementDbContext>
{
    public LibraryManagementDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var connectionString = configuration.GetConnectionString(LibraryManagementDbProperties.ConnectionStringName);

        // Fallback to hardcoded connection string if not found in configuration
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = GetDefaultConnectionString();
        }

        var builder = new DbContextOptionsBuilder<LibraryManagementDbContext>()
            .UseSqlServer(
                connectionString,
                b =>
                {
                    b.MigrationsHistoryTable("__EFMigrationsHistory");
                });

        return new LibraryManagementDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true); // Made optional

        return builder.Build();
    }

    private static string GetDefaultConnectionString()
    {
        // Default connection string for design-time migrations
        // IMPORTANT: Change this to match your local SQL Server configuration
        return "Server=DESKTOP-NP9TQ4B\\SQLEXPRESS;Database=LibraryManagementDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";
    }
}
