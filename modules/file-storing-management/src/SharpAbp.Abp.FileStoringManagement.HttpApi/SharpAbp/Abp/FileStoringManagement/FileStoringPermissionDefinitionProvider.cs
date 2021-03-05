using SharpAbp.Abp.FileStoringManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var fileStoringGroup = context.AddGroup("FileStoring", LocalizableString.Create<FileStoringManagementResource>("FileStoring"));

            var fileStoringManagement = fileStoringGroup.AddPermission(
                FileStoringPermissionConsts.FileStoringManagement,
                LocalizableString.Create<FileStoringManagementResource>($"Permission:{FileStoringPermissionConsts.FileStoringManagement}"),
                MultiTenancySides.Both);

            fileStoringManagement.AddChild(
                FileStoringPermissionConsts.ListContainer,
                LocalizableString.Create<FileStoringManagementResource>($"Permission:{FileStoringPermissionConsts.ListContainer}"),
                MultiTenancySides.Both);

            fileStoringManagement.AddChild(
                FileStoringPermissionConsts.CreateContainer,
                LocalizableString.Create<FileStoringManagementResource>($"Permission:{FileStoringPermissionConsts.CreateContainer}"),
                MultiTenancySides.Both);

            fileStoringManagement.AddChild(
                FileStoringPermissionConsts.UpdateContainer,
                LocalizableString.Create<FileStoringManagementResource>($"Permission:{FileStoringPermissionConsts.UpdateContainer}"),
                MultiTenancySides.Both);

            fileStoringManagement.AddChild(
                FileStoringPermissionConsts.DeleteContainer,
                LocalizableString.Create<FileStoringManagementResource>($"Permission:{FileStoringPermissionConsts.DeleteContainer}"),
                MultiTenancySides.Both);

            fileStoringManagement.AddChild(
                FileStoringPermissionConsts.ListProvider,
                LocalizableString.Create<FileStoringManagementResource>($"Permission:{FileStoringPermissionConsts.ListProvider}"), 
                MultiTenancySides.Both);

            fileStoringManagement.AddChild(
                FileStoringPermissionConsts.GetProvierOptions,
                LocalizableString.Create<FileStoringManagementResource>($"Permission:{FileStoringPermissionConsts.GetProvierOptions}"),
                MultiTenancySides.Both);

        }
    }
}
