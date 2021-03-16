using SharpAbp.Abp.MapTenancyManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenancyManagementPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var mapTenancyGroup = context.AddGroup(MapTenancyManagementPermissions.GroupName, L(MapTenancyManagementPermissions.GroupName));

            var mapTenantPermission = mapTenancyGroup.AddPermission(
                MapTenancyManagementPermissions.MapTenants.Default,
                L($"Permission:{MapTenancyManagementPermissions.MapTenants.Default}"));

            mapTenantPermission.AddChild(
                MapTenancyManagementPermissions.MapTenants.Create,
                L($"Permission:{MapTenancyManagementPermissions.MapTenants.Create}"));

            mapTenantPermission.AddChild(
                MapTenancyManagementPermissions.MapTenants.Update,
                L($"Permission:{MapTenancyManagementPermissions.MapTenants.Update}"));

            mapTenantPermission.AddChild(
                MapTenancyManagementPermissions.MapTenants.Delete,
                L($"Permission:{MapTenancyManagementPermissions.MapTenants.Delete}"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<MapTenancyManagementResource>(name);
        }
    }
}
