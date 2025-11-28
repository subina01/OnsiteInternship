using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Threading;

namespace LibraryManagement.Domain.Shared;

public static class LibraryManagementGlobalFeatureConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            // Configure global features here
            // Example: GlobalFeatureManager.Instance.Enable<YourFeature>();
        });
    }
}
