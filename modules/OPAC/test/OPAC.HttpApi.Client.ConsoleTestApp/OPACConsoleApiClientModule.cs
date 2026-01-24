using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace OPAC;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(OPACHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class OPACConsoleApiClientModule : AbpModule
{

}
