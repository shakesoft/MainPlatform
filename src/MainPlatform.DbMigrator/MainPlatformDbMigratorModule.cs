using MainPlatform.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using OPAC.EntityFrameworkCore;

namespace MainPlatform.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MainPlatformEntityFrameworkCoreModule),
    typeof(MainPlatformApplicationContractsModule),
    typeof(OPACEntityFrameworkCoreModule)
)]
public class MainPlatformDbMigratorModule : AbpModule
{
}
