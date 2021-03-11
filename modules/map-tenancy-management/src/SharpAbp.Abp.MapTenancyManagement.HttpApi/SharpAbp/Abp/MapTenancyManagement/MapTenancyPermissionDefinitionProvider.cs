using SharpAbp.Abp.MapTenancyManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenancyPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var mapTenancyGroup = context.AddGroup("MapTenancy", LocalizableString.Create<MapTenancyManagementResource>("MapTenancyManagement"));

            var mapTenancyManagement = mapTenancyGroup.AddPermission(
                MapTenancyPermissionConsts.MapTenancyManagement,
                LocalizableString.Create<MapTenancyManagementResource>($"Permission:{MapTenancyPermissionConsts.MapTenancyManagement}"),
                MultiTenancySides.Both);

            mapTenancyManagement.AddChild(
                MapTenancyPermissionConsts.ListMapTenant,
                LocalizableString.Create<MapTenancyManagementResource>($"Permission:{MapTenancyPermissionConsts.ListMapTenant}"),
                MultiTenancySides.Both);

            mapTenancyManagement.AddChild(
                MapTenancyPermissionConsts.CreateMapTenant,
                LocalizableString.Create<MapTenancyManagementResource>($"Permission:{MapTenancyPermissionConsts.CreateMapTenant}"),
                MultiTenancySides.Both);

            mapTenancyManagement.AddChild(
                MapTenancyPermissionConsts.UpdateMapTenant,
                LocalizableString.Create<MapTenancyManagementResource>($"Permission:{MapTenancyPermissionConsts.UpdateMapTenant}"),
                MultiTenancySides.Both);

            mapTenancyManagement.AddChild(
                MapTenancyPermissionConsts.DeleteMapTenant,
                LocalizableString.Create<MapTenancyManagementResource>($"Permission:{MapTenancyPermissionConsts.DeleteMapTenant}"),
                MultiTenancySides.Both);
        }
    }
}
