using Volo.Abp.Modularity;

namespace Collaboration;

[DependsOn(
    typeof(CollaborationDomainModule),
    typeof(CollaborationTestBaseModule)
)]
public class CollaborationDomainTestModule : AbpModule
{

}
