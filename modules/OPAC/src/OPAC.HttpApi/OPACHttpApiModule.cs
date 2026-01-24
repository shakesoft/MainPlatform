using Localization.Resources.AbpUi;
using OPAC.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace OPAC;

[DependsOn(
    typeof(OPACApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class OPACHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(OPACHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<OPACResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
