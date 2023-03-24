using SharpAbp.Abp.TenantGroupManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupManagementPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var tenantGroup = context.AddGroup(TenantGroupManagementPermissions.GroupName, L(TenantGroupManagementPermissions.GroupName));

            var tenantGroupPermission = tenantGroup.AddPermission(
                TenantGroupManagementPermissions.TenantGroups.Default,
                L($"Permission:{TenantGroupManagementPermissions.TenantGroups.Default}"));

            tenantGroupPermission.AddChild(
                TenantGroupManagementPermissions.TenantGroups.Create,
                L($"Permission:{TenantGroupManagementPermissions.TenantGroups.Create}"));

            tenantGroupPermission.AddChild(
                TenantGroupManagementPermissions.TenantGroups.Update,
                L($"Permission:{TenantGroupManagementPermissions.TenantGroups.Update}"));

            tenantGroupPermission.AddChild(
                TenantGroupManagementPermissions.TenantGroups.Delete,
                L($"Permission:{TenantGroupManagementPermissions.TenantGroups.Delete}"));

            tenantGroupPermission.AddChild(
                TenantGroupManagementPermissions.TenantGroups.ManageFeatures,
                L($"Permission:{TenantGroupManagementPermissions.TenantGroups.ManageFeatures}"));

            tenantGroupPermission.AddChild(
                TenantGroupManagementPermissions.TenantGroups.ManageConnectionStrings,
                L($"Permission:{TenantGroupManagementPermissions.TenantGroups.ManageConnectionStrings}"));
        }


        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<TenantGroupManagementResource>(name);
        }
    }
}
