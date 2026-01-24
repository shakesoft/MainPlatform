using OPAC.Localization;
using Volo.Abp.Application.Services;

namespace OPAC;

public abstract class OPACAppService : ApplicationService
{
    protected OPACAppService()
    {
        LocalizationResource = typeof(OPACResource);
        ObjectMapperContext = typeof(OPACApplicationModule);
    }
}
