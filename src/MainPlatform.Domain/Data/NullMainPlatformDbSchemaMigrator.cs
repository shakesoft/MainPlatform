using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MainPlatform.Data;

/* This is used if database provider does't define
 * IMainPlatformDbSchemaMigrator implementation.
 */
public class NullMainPlatformDbSchemaMigrator : IMainPlatformDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
