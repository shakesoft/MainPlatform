using Volo.Abp.Modularity;

namespace OPAC;

[DependsOn(
    typeof(OPACApplicationModule),
    typeof(OPACDomainTestModule)
    )]
public class OPACApplicationTestModule : AbpModule
{

}
