using Volo.Abp.Modularity;

namespace Collaboration;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class CollaborationApplicationTestBase<TStartupModule> : CollaborationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
