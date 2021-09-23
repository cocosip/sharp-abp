using SharpAbp.Abp.AuditLogging.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace SharpAbp.Abp.AuditLogging
{
    public class AuditLoggingPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var auditLoggingGroup = context.AddGroup(AuditLoggingPermissions.GroupName, L(AuditLoggingPermissions.GroupName));

            var auditLogPermission = auditLoggingGroup.AddPermission(
                AuditLoggingPermissions.AuditLogs.Default,
                L($"Permission:{AuditLoggingPermissions.AuditLogs.Default}"));

            auditLogPermission.AddChild(
                AuditLoggingPermissions.AuditLogs.Create,
                L($"Permission:{AuditLoggingPermissions.AuditLogs.Create}"));

            auditLogPermission.AddChild(
                AuditLoggingPermissions.AuditLogs.Update,
                L($"Permission:{AuditLoggingPermissions.AuditLogs.Update}"));

            auditLogPermission.AddChild(
                AuditLoggingPermissions.AuditLogs.Delete,
                L($"Permission:{AuditLoggingPermissions.AuditLogs.Delete}"));
        }


        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AuditLoggingResource>(name);
        }
    }
}
