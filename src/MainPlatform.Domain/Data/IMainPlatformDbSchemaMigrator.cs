using System.Threading.Tasks;

namespace MainPlatform.Data;

public interface IMainPlatformDbSchemaMigrator
{
    Task MigrateAsync();
}
