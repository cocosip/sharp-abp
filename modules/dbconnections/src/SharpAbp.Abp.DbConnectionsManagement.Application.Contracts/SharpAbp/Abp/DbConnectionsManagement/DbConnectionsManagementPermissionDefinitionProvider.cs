using SharpAbp.Abp.DbConnectionsManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DbConnectionsManagementPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var dbConnectionsManagementGroup = context.AddGroup(DbConnectionsManagementPermissions.GroupName, L(DbConnectionsManagementPermissions.GroupName));

            var databaseConnectionInfoPermission = dbConnectionsManagementGroup.AddPermission(
                DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default,
                L($"Permission:{DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default}"));

            databaseConnectionInfoPermission.AddChild(
                DbConnectionsManagementPermissions.DatabaseConnectionInfos.Create,
                L($"Permission:{DbConnectionsManagementPermissions.DatabaseConnectionInfos.Create}"));

            databaseConnectionInfoPermission.AddChild(
                DbConnectionsManagementPermissions.DatabaseConnectionInfos.Update,
                L($"Permission:{DbConnectionsManagementPermissions.DatabaseConnectionInfos.Update}"));

            databaseConnectionInfoPermission.AddChild(
                DbConnectionsManagementPermissions.DatabaseConnectionInfos.Delete,
                L($"Permission:{DbConnectionsManagementPermissions.DatabaseConnectionInfos.Delete}"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<DbConnectionsManagementResource>(name);
        }
    }
}