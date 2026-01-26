using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Application;

namespace Collaboration;

[DependsOn(
    typeof(CollaborationDomainModule),
    typeof(CollaborationApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule)
    )]
public class CollaborationApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<CollaborationApplicationModule>();
    }
}
