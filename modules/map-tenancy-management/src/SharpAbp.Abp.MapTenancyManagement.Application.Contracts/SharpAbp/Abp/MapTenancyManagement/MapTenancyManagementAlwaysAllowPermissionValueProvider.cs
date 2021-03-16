using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenancyManagementAlwaysAllowPermissionValueProvider : PermissionValueProvider
    {
        public MapTenancyManagementAlwaysAllowPermissionValueProvider(IPermissionStore permissionStore)
            : base(permissionStore)
        {

        }

        public override string Name => MapTenancyManagementPermissions.AlwaysAllow;

        public override Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
        {
            var permissions = MapTenancyManagementPermissions.GetAll();
            if (permissions.Contains(context.Permission.Name))
            {
                return Task.FromResult(PermissionGrantResult.Granted);
            }
            return Task.FromResult(PermissionGrantResult.Prohibited);
        }

        public override Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
        {
            var permissions = MapTenancyManagementPermissions.GetAll();
            var result = new MultiplePermissionGrantResult();
            foreach (var permissionDefinition in context.Permissions)
            {
                if (permissions.Contains(permissionDefinition.Name))
                {
                    result.Result.Add(permissionDefinition.Name, PermissionGrantResult.Granted);
                }
                else
                {
                    result.Result.Add(permissionDefinition.Name, PermissionGrantResult.Prohibited);
                }
            }
            return Task.FromResult(result);
        }
    }
}
