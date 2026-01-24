using MainPlatform.Localization;
using Volo.Abp.AspNetCore.Components;

namespace MainPlatform.Blazor;

public abstract class MainPlatformComponentBase : AbpComponentBase
{
    protected MainPlatformComponentBase()
    {
        LocalizationResource = typeof(MainPlatformResource);
    }
}
