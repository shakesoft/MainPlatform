using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.Modularity;

namespace OPAC.Blazor.Server;

[DependsOn(
    typeof(AbpAspNetCoreComponentsServerThemingModule),
    typeof(OPACBlazorModule)
    )]
public class OPACBlazorServerModule : AbpModule
{

}
