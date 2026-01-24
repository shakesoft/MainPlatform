using OPAC.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace OPAC.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class OPACPageModel : AbpPageModel
{
    protected OPACPageModel()
    {
        LocalizationResourceType = typeof(OPACResource);
        ObjectMapperContext = typeof(OPACWebModule);
    }
}
