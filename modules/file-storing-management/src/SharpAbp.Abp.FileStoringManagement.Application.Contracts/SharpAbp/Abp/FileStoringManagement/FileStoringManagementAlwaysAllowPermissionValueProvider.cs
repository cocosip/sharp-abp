using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringManagementAlwaysAllowPermissionValueProvider : PermissionValueProvider
    {
        public FileStoringManagementAlwaysAllowPermissionValueProvider(IPermissionStore permissionStore)
            : base(permissionStore)
        {
        }

        public override string Name => FileStoringManagementPermissions.AlwaysAllow;

        public override Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
        {
            var permissions = FileStoringManagementPermissions.GetAll();
            if (permissions.Contains(context.Permission.Name))
            {
                return Task.FromResult(PermissionGrantResult.Granted);
            }
            return Task.FromResult(PermissionGrantResult.Prohibited);
        }

        public override Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
        {
            var permissions = FileStoringManagementPermissions.GetAll();
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
