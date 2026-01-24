using Xunit;

namespace MainPlatform.EntityFrameworkCore;

[CollectionDefinition(MainPlatformTestConsts.CollectionDefinitionName)]
public class MainPlatformEntityFrameworkCoreCollection : ICollectionFixture<MainPlatformEntityFrameworkCoreFixture>
{

}
