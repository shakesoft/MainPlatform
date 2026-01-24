using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace OPAC;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(OPACDomainSharedModule)
)]
public class OPACDomainModule : AbpModule
{

}
