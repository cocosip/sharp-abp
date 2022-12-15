using SharpSample.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace SharpSample.Permissions;

public class SharpSamplePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(SharpSamplePermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(SharpSamplePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SharpSampleResource>(name);
    }
}
