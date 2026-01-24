using Volo.Abp.Modularity;

namespace MainPlatform;

[DependsOn(
    typeof(MainPlatformApplicationModule),
    typeof(MainPlatformDomainTestModule)
)]
public class MainPlatformApplicationTestModule : AbpModule
{

}
