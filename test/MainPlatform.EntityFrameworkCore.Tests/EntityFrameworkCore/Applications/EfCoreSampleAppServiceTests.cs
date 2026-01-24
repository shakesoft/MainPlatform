using MainPlatform.Samples;
using Xunit;

namespace MainPlatform.EntityFrameworkCore.Applications;

[Collection(MainPlatformTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<MainPlatformEntityFrameworkCoreTestModule>
{

}
