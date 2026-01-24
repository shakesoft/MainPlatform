using Volo.Abp.Modularity;

namespace OPAC;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class OPACDomainTestBase<TStartupModule> : OPACTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
