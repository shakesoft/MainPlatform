using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MainPlatform.Data;
using Volo.Abp.DependencyInjection;

namespace MainPlatform.EntityFrameworkCore;

public class EntityFrameworkCoreMainPlatformDbSchemaMigrator
    : IMainPlatformDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreMainPlatformDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the MainPlatformDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<MainPlatformDbContext>()
            .Database
            .MigrateAsync();
    }
}
