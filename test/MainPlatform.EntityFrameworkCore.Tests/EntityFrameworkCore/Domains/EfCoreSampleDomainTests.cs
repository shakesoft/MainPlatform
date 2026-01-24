using MainPlatform.Samples;
using Xunit;

namespace MainPlatform.EntityFrameworkCore.Domains;

[Collection(MainPlatformTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<MainPlatformEntityFrameworkCoreTestModule>
{

}
