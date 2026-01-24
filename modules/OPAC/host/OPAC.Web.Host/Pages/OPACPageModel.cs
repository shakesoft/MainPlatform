using OPAC.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace OPAC.Pages;

public abstract class OPACPageModel : AbpPageModel
{
    protected OPACPageModel()
    {
        LocalizationResourceType = typeof(OPACResource);
    }
}
