using SharpAbp.Abp.FileStoringManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringManagementPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var fileStoringGroup = context.AddGroup(FileStoringManagementPermissions.GroupName, L(FileStoringManagementPermissions.GroupName));

            var containerPermission = fileStoringGroup.AddPermission(
                FileStoringManagementPermissions.Containers.Default,
                L($"Permission:{FileStoringManagementPermissions.Containers.Default}"));

            containerPermission.AddChild(
                FileStoringManagementPermissions.Containers.Create,
                L($"Permission:{FileStoringManagementPermissions.Containers.Create}"));

            containerPermission.AddChild(
                FileStoringManagementPermissions.Containers.Update,
                L($"Permission:{FileStoringManagementPermissions.Containers.Update}"));

            containerPermission.AddChild(
                FileStoringManagementPermissions.Containers.Delete,
                L($"Permission:{FileStoringManagementPermissions.Containers.Delete}"));

            var providerPermission = fileStoringGroup.AddPermission(
                FileStoringManagementPermissions.Providers.Default,
                L($"Permission:{FileStoringManagementPermissions.Providers.Default}"));

            providerPermission.AddChild(
                FileStoringManagementPermissions.Providers.Options,
                L($"Permission:{FileStoringManagementPermissions.Providers.Options}"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<FileStoringManagementResource>(name);
        }
    }
}
