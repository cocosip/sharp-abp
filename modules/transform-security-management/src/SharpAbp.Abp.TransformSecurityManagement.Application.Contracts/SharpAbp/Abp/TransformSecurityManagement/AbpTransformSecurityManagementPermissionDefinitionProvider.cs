using SharpAbp.Abp.TransformSecurityManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public class AbpTransformSecurityManagementPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var transformSecurityManagementGroup = context.AddGroup(TransformSecurityManagementPermissions.GroupName, L(TransformSecurityManagementPermissions.GroupName));

            var securityCredentialInfoPermission = transformSecurityManagementGroup.AddPermission(
                TransformSecurityManagementPermissions.SecurityCredentialInfos.Default,
                L($"Permission:{TransformSecurityManagementPermissions.SecurityCredentialInfos.Default}"));

            securityCredentialInfoPermission.AddChild(
                TransformSecurityManagementPermissions.SecurityCredentialInfos.Create,
                L($"Permission:{TransformSecurityManagementPermissions.SecurityCredentialInfos.Create}"));

            securityCredentialInfoPermission.AddChild(
                TransformSecurityManagementPermissions.SecurityCredentialInfos.Update,
                L($"Permission:{TransformSecurityManagementPermissions.SecurityCredentialInfos.Update}"));

            securityCredentialInfoPermission.AddChild(
                TransformSecurityManagementPermissions.SecurityCredentialInfos.Delete,
                L($"Permission:{TransformSecurityManagementPermissions.SecurityCredentialInfos.Delete}"));

        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AbpTransformSecurityManagementResource>(name);
        }
    }
}
