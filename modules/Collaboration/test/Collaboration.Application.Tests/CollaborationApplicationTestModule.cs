using Volo.Abp.Modularity;

namespace Collaboration;

[DependsOn(
    typeof(CollaborationApplicationModule),
    typeof(CollaborationDomainTestModule)
    )]
public class CollaborationApplicationTestModule : AbpModule
{

}
