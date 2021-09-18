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
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<IdentityResource>(name);
        }
    }
}
