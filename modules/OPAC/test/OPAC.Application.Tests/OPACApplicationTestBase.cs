using Volo.Abp.Modularity;

namespace OPAC;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class OPACApplicationTestBase<TStartupModule> : OPACTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
