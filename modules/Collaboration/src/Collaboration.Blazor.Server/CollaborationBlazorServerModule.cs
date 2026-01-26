using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.Modularity;

namespace Collaboration.Blazor.Server;

[DependsOn(
    typeof(AbpAspNetCoreComponentsServerThemingModule),
    typeof(CollaborationBlazorModule)
    )]
public class CollaborationBlazorServerModule : AbpModule
{

}
