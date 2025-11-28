using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using TodoApp.Localization;

namespace TodoApp.Web;

[Dependency(ReplaceServices = true)]
public class TodoAppBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<TodoAppResource> _localizer;

    public TodoAppBrandingProvider(IStringLocalizer<TodoAppResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
