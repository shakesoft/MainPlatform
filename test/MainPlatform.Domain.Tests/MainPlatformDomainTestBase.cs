using Volo.Abp.Modularity;

namespace MainPlatform;

/* Inherit from this class for your domain layer tests. */
public abstract class MainPlatformDomainTestBase<TStartupModule> : MainPlatformTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
