using Microsoft.Extensions.Localization;
using MainPlatform.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MainPlatform;

[Dependency(ReplaceServices = true)]
public class MainPlatformBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MainPlatformResource> _localizer;

    public MainPlatformBrandingProvider(IStringLocalizer<MainPlatformResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
