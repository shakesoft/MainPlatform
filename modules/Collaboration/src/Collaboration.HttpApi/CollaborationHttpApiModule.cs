using Localization.Resources.AbpUi;
using Collaboration.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Collaboration;

[DependsOn(
    typeof(CollaborationApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class CollaborationHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CollaborationHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<CollaborationResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
