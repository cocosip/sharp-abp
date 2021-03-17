using SharpAbp.WebSample.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace SharpAbp.WebSample.Permissions
{
    public class WebSamplePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(WebSamplePermissions.GroupName);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(WebSamplePermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<WebSampleResource>(name);
        }
    }
}
