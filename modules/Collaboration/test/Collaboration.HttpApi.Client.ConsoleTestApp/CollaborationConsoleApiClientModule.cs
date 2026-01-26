using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Collaboration;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(CollaborationHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class CollaborationConsoleApiClientModule : AbpModule
{

}
