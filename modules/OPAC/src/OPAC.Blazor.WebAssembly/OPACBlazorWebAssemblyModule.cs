using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace OPAC.Blazor.WebAssembly;

[DependsOn(
    typeof(OPACBlazorModule),
    typeof(OPACHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class OPACBlazorWebAssemblyModule : AbpModule
{

}
