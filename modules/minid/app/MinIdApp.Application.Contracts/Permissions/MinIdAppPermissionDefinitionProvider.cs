using MinIdApp.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MinIdApp.Permissions
{
    public class MinIdAppPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(MinIdAppPermissions.GroupName);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(MinIdAppPermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<MinIdAppResource>(name);
        }
    }
}
