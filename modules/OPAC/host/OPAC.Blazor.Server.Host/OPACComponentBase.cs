using OPAC.Localization;
using Volo.Abp.AspNetCore.Components;

namespace OPAC.Blazor.Server.Host;

public abstract class OPACComponentBase : AbpComponentBase
{
    protected OPACComponentBase()
    {
        LocalizationResource = typeof(OPACResource);
    }
}
