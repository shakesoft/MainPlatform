using Volo.Abp.Modularity;

namespace OPAC;

[DependsOn(
    typeof(OPACDomainModule),
    typeof(OPACTestBaseModule)
)]
public class OPACDomainTestModule : AbpModule
{

}
