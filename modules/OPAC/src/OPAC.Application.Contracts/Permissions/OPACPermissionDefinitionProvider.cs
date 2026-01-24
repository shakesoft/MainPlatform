using OPAC.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace OPAC.Permissions;

public class OPACPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(OPACPermissions.GroupName, L("Permission:OPAC"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<OPACResource>(name);
    }
}
