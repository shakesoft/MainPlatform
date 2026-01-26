using Collaboration.Localization;
using Volo.Abp.Application.Services;

namespace Collaboration;

public abstract class CollaborationAppService : ApplicationService
{
    protected CollaborationAppService()
    {
        LocalizationResource = typeof(CollaborationResource);
        ObjectMapperContext = typeof(CollaborationApplicationModule);
    }
}
