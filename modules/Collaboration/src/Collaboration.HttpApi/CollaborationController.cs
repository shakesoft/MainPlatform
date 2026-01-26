using Collaboration.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Collaboration;

public abstract class CollaborationController : AbpControllerBase
{
    protected CollaborationController()
    {
        LocalizationResource = typeof(CollaborationResource);
    }
}
