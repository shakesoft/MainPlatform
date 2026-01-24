using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace OPAC;

[DependsOn(
    typeof(OPACApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class OPACHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(OPACApplicationContractsModule).Assembly,
            OPACRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<OPACHttpApiClientModule>();
        });

    }
}
