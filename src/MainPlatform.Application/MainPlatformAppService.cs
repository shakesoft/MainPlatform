using MainPlatform.Localization;
using Volo.Abp.Application.Services;

namespace MainPlatform;

/* Inherit your application services from this class.
 */
public abstract class MainPlatformAppService : ApplicationService
{
    protected MainPlatformAppService()
    {
        LocalizationResource = typeof(MainPlatformResource);
    }
}
