using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Account;
using Volo.Abp.Authorization;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace LibraryManagement.Application.Contracts;

[DependsOn(
    typeof(LibraryManagementDomainSharedModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpObjectExtendingModule),
    typeof(AbpIdentityApplicationContractsModule),
    typeof(AbpAccountApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(AbpTenantManagementApplicationContractsModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule)
)]
public class LibraryManagementApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        LibraryManagementDtoExtensions.Configure();
    }
}
