using LibraryManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using LibraryManagement.Domain.Entities;
using LibraryManagement.EntityFrameworkCore.EntityFrameworkCore;

namespace LibraryManagement.EntityFrameworkCore;

[DependsOn(
    typeof(LibraryManagementDomainModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule)
)]
public class LibraryManagementEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<LibraryManagementDbContext>(options =>
        {
            // Add custom repositories with specific implementations
            options.AddRepository<Book, Repositories.EfCoreBookRepository>();
            options.AddRepository<Author, Repositories.EfCoreAuthorRepository>();
            options.AddRepository<Category, Repositories.EfCoreCategoryRepository>();
            options.AddRepository<Publisher, Repositories.EfCorePublisherRepository>();
            options.AddRepository<Member,Repositories.EfCoreMemberRepository>();
            options.AddRepository<Loan, Repositories.EfCoreLoanRepository>();
            options.AddRepository<Reservation,Repositories.EfCoreReservationRepository>();

            // Add default repositories for remaining entities
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * Use UseSqlServer for SQL Server
             * Use UseNpgsql for PostgreSQL
             * Use UseMySql for MySQL
             * Use UseSqlite for SQLite
             * Use UseInMemoryDatabase for In-Memory database
             */
            options.UseSqlServer();
        });
    }
}