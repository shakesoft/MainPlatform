using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace Collaboration;

[DependsOn(
    typeof(CollaborationDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class CollaborationApplicationContractsModule : AbpModule
{

}
