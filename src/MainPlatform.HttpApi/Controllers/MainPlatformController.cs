using MainPlatform.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MainPlatform.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MainPlatformController : AbpControllerBase
{
    protected MainPlatformController()
    {
        LocalizationResource = typeof(MainPlatformResource);
    }
}
