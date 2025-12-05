using Common.Domain;
using Identity.Domain.Entities;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Infrastructure.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Seed default roles
        SeedRoles(modelBuilder);
    }

    private void SeedRoles(ModelBuilder modelBuilder)
    {
        // Use static GUIDs to avoid migration issues
        var adminRoleId = new Guid("11111111-1111-1111-1111-111111111111");
        var customerRoleId = new Guid("22222222-2222-2222-2222-222222222222");
        var managerRoleId = new Guid("33333333-3333-3333-3333-333333333333");
        var adminUserId = new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        var createdAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Seed roles
        modelBuilder.Entity<Role>().HasData(
            new
            {
                Id = adminRoleId,
                Name = RoleNames.Administrator,
                Description = "Full system access",
                CreatedAt = createdAt,
                CreatedBy = "System",
                IsDeleted = false
            },
            new
            {
                Id = customerRoleId,
                Name = RoleNames.Customer,
                Description = "Regular customer access",
                CreatedAt = createdAt,
                CreatedBy = "System",
                IsDeleted = false
            },
            new
            {
                Id = managerRoleId,
                Name = RoleNames.Manager,
                Description = "Management access",
                CreatedAt = createdAt,
                CreatedBy = "System",
                IsDeleted = false
            }
        );

        // Seed admin user
        // Password hash for "Admin123!" - using deterministic hash with fixed salt for seeding
        // Note: In production, you'd want to change this password after first login
        var adminPasswordHash = GenerateSeededPasswordHash("Admin123!");

        modelBuilder.Entity<ApplicationUser>().HasData(
            new
            {
                Id = adminUserId,
                UserName = "admin",
                Email = "admin@ecommerce.com",
                PasswordHash = adminPasswordHash,
                FirstName = "System",
                LastName = "Administrator",
                PhoneNumber = "1234567890",
                IsEmailConfirmed = true,
                IsPhoneNumberConfirmed = false,
                IsActive = true,
                LastLoginDate = (DateTime?)null,
                FailedLoginAttempts = 0,
                LockoutEnd = (DateTime?)null,
                CreatedAt = createdAt,
                CreatedBy = "System",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false
            }
        );

        // Seed admin user role assignment
        modelBuilder.Entity<UserRole>().HasData(
            new
            {
                UserId = adminUserId,
                RoleId = adminRoleId
            }
        );
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEventsAsync()
    {
        var domainEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());
    }

    /// <summary>
    /// Generates a deterministic password hash for seeding purposes
    /// Uses a fixed salt for consistency across migrations
    /// </summary>
    private static string GenerateSeededPasswordHash(string password)
    {
        // Use a fixed salt for seeding (must be consistent across migrations)
        string fixedSaltString = "SeedSalt1234567890123456"; // 24 characters
        byte[] salt = Encoding.UTF8.GetBytes(fixedSaltString);

        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8);

        // Format: salt.hash (matching the PasswordHasher format)
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }
}
