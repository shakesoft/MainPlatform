using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace OPAC;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class OPACInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<OPACInstallerModule>();
        });
    }
}
