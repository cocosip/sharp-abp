using FileStoringSample.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace FileStoringSample.Permissions
{
    public class FileStoringSamplePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(FileStoringSamplePermissions.GroupName);
            //Define your own permissions here. Example:
            //myGroup.AddPermission(FileStoringSamplePermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<FileStoringSampleResource>(name);
        }
    }
}
