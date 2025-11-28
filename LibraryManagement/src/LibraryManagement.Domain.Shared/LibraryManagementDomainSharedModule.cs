using LibraryManagement.Domain.Shared;
using LibraryManagement.Domain.Shared.Localization.LibraryManagement;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

[DependsOn(
    typeof(AbpValidationModule),
    typeof(AbpLocalizationModule),
    typeof(AbpIdentityDomainSharedModule)
)]
public class LibraryManagementDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        LibraryManagementGlobalFeatureConfigurator.Configure();
        LibraryManagementModuleExtensionConfigurator.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<LibraryManagementDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<LibraryManagementResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/LibraryManagement");

            options.DefaultResourceType = typeof(LibraryManagementResource);
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("LibraryManagement", typeof(LibraryManagementResource));
        });
    }
}
