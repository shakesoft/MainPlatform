using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace OPAC;

[Dependency(ReplaceServices = true)]
public class OPACBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "OPAC";
}
