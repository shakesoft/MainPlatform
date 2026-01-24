using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace OPAC.Blazor.Server.Host;

[Dependency(ReplaceServices = true)]
public class OPACBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "OPAC";
}
