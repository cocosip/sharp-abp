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
            var mapTenancyGroup = context.AddGroup(MapTenancyManagementPermissions.GroupName, L($"Permission:{MapTenancyManagementPermissions.MapTenants.Default}"));

            var mapTenantPermission = mapTenancyGroup.AddPermission(
                MapTenancyManagementPermissions.MapTenants.Default,
                L($"Permission:{MapTenancyManagementPermissions.MapTenants.Default}"),
                MultiTenancySides.Both);

            mapTenantPermission.AddChild(
                MapTenancyManagementPermissions.MapTenants.Create,
                L($"Permission:{MapTenancyManagementPermissions.MapTenants.Create}"),
                MultiTenancySides.Both);

            mapTenantPermission.AddChild(
                MapTenancyManagementPermissions.MapTenants.Update,
                L($"Permission:{MapTenancyManagementPermissions.MapTenants.Update}"),
                MultiTenancySides.Both);

            mapTenantPermission.AddChild(
                MapTenancyManagementPermissions.MapTenants.Delete,
                L($"Permission:{MapTenancyManagementPermissions.MapTenants.Delete}"),
                MultiTenancySides.Both);
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<MapTenancyManagementResource>(name);
        }
    }
}
