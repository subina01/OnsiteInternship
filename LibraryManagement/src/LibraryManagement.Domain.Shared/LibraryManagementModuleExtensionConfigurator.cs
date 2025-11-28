using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Threading;

namespace LibraryManagement.Domain.Shared;

public static class LibraryManagementModuleExtensionConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            ConfigureExistingProperties();
            ConfigureExtraProperties();
        });
    }

    private static void ConfigureExistingProperties()
    {
        // Configure existing properties here
        // Example: ObjectExtensionManager.Instance.MapEfCoreProperty<IdentityUser, string>("SocialSecurityNumber");
    }

    private static void ConfigureExtraProperties()
    {
        // Configure extra properties here
        // Example: ObjectExtensionManager.Instance.AddOrUpdateProperty<IdentityUser, string>("SocialSecurityNumber");
    }
}
