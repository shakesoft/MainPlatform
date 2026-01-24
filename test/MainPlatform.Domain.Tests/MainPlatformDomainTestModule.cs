using Volo.Abp.Modularity;

namespace MainPlatform;

[DependsOn(
    typeof(MainPlatformDomainModule),
    typeof(MainPlatformTestBaseModule)
)]
public class MainPlatformDomainTestModule : AbpModule
{

}
