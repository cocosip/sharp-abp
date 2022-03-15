using AllApiSample.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace AllApiSample.Permissions;

public class AllApiSamplePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(AllApiSamplePermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(AllApiSamplePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AllApiSampleResource>(name);
    }
}
