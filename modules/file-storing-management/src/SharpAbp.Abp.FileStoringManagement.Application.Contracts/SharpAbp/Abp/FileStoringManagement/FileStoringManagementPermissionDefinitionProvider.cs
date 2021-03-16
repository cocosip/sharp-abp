using SharpAbp.Abp.FileStoringManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringManagementPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var fileStoringGroup = context.AddGroup(FileStoringManagementPermissions.GroupName, L($"Permission:{FileStoringManagementPermissions.Containers.Default}"));

            var containerPermission = fileStoringGroup.AddPermission(
                FileStoringManagementPermissions.Containers.Default,
                L($"Permission:{FileStoringManagementPermissions.Containers.Default}"),
                MultiTenancySides.Both);

            containerPermission.AddChild(
                FileStoringManagementPermissions.Containers.Create,
                L($"Permission:{FileStoringManagementPermissions.Containers.Create}"),
                MultiTenancySides.Both);

            containerPermission.AddChild(
                FileStoringManagementPermissions.Containers.Update,
                L($"Permission:{FileStoringManagementPermissions.Containers.Update}"),
                MultiTenancySides.Both);

            containerPermission.AddChild(
                FileStoringManagementPermissions.Containers.Delete,
                L($"Permission:{FileStoringManagementPermissions.Containers.Delete}"),
                MultiTenancySides.Both);

            var providerPermission = fileStoringGroup.AddPermission(
                FileStoringManagementPermissions.Providers.Default,
                L($"Permission:{FileStoringManagementPermissions.Providers.Default}"),
                MultiTenancySides.Both);

            providerPermission.AddChild(
                FileStoringManagementPermissions.Providers.Options,
                L($"Permission:{FileStoringManagementPermissions.Providers.Options}"),
                MultiTenancySides.Both);
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<FileStoringManagementResource>(name);
        }
    }
}
