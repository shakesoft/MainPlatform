using OPAC.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace OPAC;

public abstract class OPACController : AbpControllerBase
{
    protected OPACController()
    {
        LocalizationResource = typeof(OPACResource);
    }
}
