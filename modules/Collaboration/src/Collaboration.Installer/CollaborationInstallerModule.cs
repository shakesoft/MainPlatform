using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Collaboration;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class CollaborationInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CollaborationInstallerModule>();
        });
    }
}
