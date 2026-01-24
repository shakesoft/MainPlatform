using Volo.Abp.Reflection;

namespace OPAC.Permissions;

public class OPACPermissions
{
    public const string GroupName = "OPAC";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(OPACPermissions));
    }
}
