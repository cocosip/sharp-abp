using IdentityModelSample.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace IdentityModelSample.Permissions
{
    public class IdentityModelSamplePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(IdentityModelSamplePermissions.GroupName);
            //Define your own permissions here. Example:
            //myGroup.AddPermission(IdentityModelSamplePermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<IdentityModelSampleResource>(name);
        }
    }
}
