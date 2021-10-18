using SharpAbp.Abp.Identity.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace SharpAbp.Abp.Identity
{
    public class IdentityPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var identityGroup = context.AddGroup(IdentityPermissions.GroupName, L(IdentityPermissions.GroupName));

            //identityClaimType
            var identityClaimTypePermission = identityGroup.AddPermission(
                IdentityPermissions.IdentityClaimTypes.Default,
                L($"Permission:{IdentityPermissions.IdentityClaimTypes.Default}"));

            identityClaimTypePermission.AddChild(
                IdentityPermissions.IdentityClaimTypes.Create,
                L($"Permission:{IdentityPermissions.IdentityClaimTypes.Create}"));

            identityClaimTypePermission.AddChild(
                IdentityPermissions.IdentityClaimTypes.Update,
                L($"Permission:{IdentityPermissions.IdentityClaimTypes.Update}"));

            identityClaimTypePermission.AddChild(
                IdentityPermissions.IdentityClaimTypes.Delete,
                L($"Permission:{IdentityPermissions.IdentityClaimTypes.Delete}"));

            //IdentitySecurityLogs
            var identitySecurityLogsPermission = identityGroup.AddPermission(
                IdentityPermissions.IdentitySecurityLogs.Default,
                L($"Permission:{IdentityPermissions.IdentitySecurityLogs.Default}"));

            identitySecurityLogsPermission.AddChild(
                IdentityPermissions.IdentitySecurityLogs.Create,
                L($"Permission:{IdentityPermissions.IdentitySecurityLogs.Create}"));

            identitySecurityLogsPermission.AddChild(
                IdentityPermissions.IdentitySecurityLogs.Update,
                L($"Permission:{IdentityPermissions.IdentitySecurityLogs.Update}"));

            identitySecurityLogsPermission.AddChild(
                IdentityPermissions.IdentitySecurityLogs.Delete,
                L($"Permission:{IdentityPermissions.IdentitySecurityLogs.Delete}"));

            //OrganizationUnits
            var organizationUnitsPermission = identityGroup.AddPermission(
                IdentityPermissions.OrganizationUnits.Default,
                L($"Permission:{IdentityPermissions.OrganizationUnits.Default}"));

            organizationUnitsPermission.AddChild(
                IdentityPermissions.OrganizationUnits.Create,
                L($"Permission:{IdentityPermissions.OrganizationUnits.Create}"));

            organizationUnitsPermission.AddChild(
                IdentityPermissions.OrganizationUnits.Update,
                L($"Permission:{IdentityPermissions.OrganizationUnits.Update}"));

            organizationUnitsPermission.AddChild(
                IdentityPermissions.OrganizationUnits.Delete,
                L($"Permission:{IdentityPermissions.OrganizationUnits.Delete}"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<IdentityResource>(name);
        }
    }
}
