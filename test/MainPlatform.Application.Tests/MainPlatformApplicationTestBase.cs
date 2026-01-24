using Volo.Abp.Modularity;

namespace MainPlatform;

public abstract class MainPlatformApplicationTestBase<TStartupModule> : MainPlatformTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
