using Volo.Abp.Reflection;

namespace Collaboration.Permissions;

public class CollaborationPermissions
{
    public const string GroupName = "Collaboration";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(CollaborationPermissions));
    }
}
