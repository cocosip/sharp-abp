using SharpAbp.Abp.OpenIddict.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace SharpAbp.Abp.OpenIddict
{
    public class OpenIddictPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var openIddictGroup = context.AddGroup(OpenIddictPermissions.GroupName, L(OpenIddictPermissions.GroupName));

            var applicationPermission = openIddictGroup.AddPermission(
                OpenIddictPermissions.Applications.Default,
                L($"Permission:{OpenIddictPermissions.Applications.Default}"));

            applicationPermission.AddChild(
                OpenIddictPermissions.Applications.Create,
                L($"Permission:{OpenIddictPermissions.Applications.Create}"));

            applicationPermission.AddChild(
                OpenIddictPermissions.Applications.Update,
                L($"Permission:{OpenIddictPermissions.Applications.Update}"));

            applicationPermission.AddChild(
                OpenIddictPermissions.Applications.Delete,
                L($"Permission:{OpenIddictPermissions.Applications.Delete}"));

            var scopePermission = openIddictGroup.AddPermission(
                OpenIddictPermissions.Scopes.Default,
                L($"Permission:{OpenIddictPermissions.Scopes.Default}"));

            scopePermission.AddChild(
                OpenIddictPermissions.Scopes.Create,
                L($"Permission:{OpenIddictPermissions.Scopes.Create}"));

            scopePermission.AddChild(
                OpenIddictPermissions.Scopes.Update,
                L($"Permission:{OpenIddictPermissions.Scopes.Update}"));

            scopePermission.AddChild(
                OpenIddictPermissions.Scopes.Delete,
                L($"Permission:{OpenIddictPermissions.Scopes.Delete}"));

        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<OpenIddictResource>(name);
        }
    }
}
