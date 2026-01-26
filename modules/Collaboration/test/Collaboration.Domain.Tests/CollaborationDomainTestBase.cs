using Volo.Abp.Modularity;

namespace Collaboration;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class CollaborationDomainTestBase<TStartupModule> : CollaborationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
