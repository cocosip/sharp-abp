using SharpAbp.Abp.IdentityServer.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace SharpAbp.Abp.IdentityServer
{
    public class IdentityServerPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var identityServerGroup = context.AddGroup(IdentityServerPermissions.GroupName, L(IdentityServerPermissions.GroupName));

            //apiResource
            var apiResourcePermission = identityServerGroup.AddPermission(
                IdentityServerPermissions.ApiResources.Default,
                L($"Permission:{IdentityServerPermissions.ApiResources.Default}"));

            apiResourcePermission.AddChild(
                IdentityServerPermissions.ApiResources.Create,
                L($"Permission:{IdentityServerPermissions.ApiResources.Create}"));

            apiResourcePermission.AddChild(
                IdentityServerPermissions.ApiResources.Update,
                L($"Permission:{IdentityServerPermissions.ApiResources.Update}"));

            apiResourcePermission.AddChild(
                IdentityServerPermissions.ApiResources.Delete,
                L($"Permission:{IdentityServerPermissions.ApiResources.Delete}"));

            //apiScope
            var apiScopePermission = identityServerGroup.AddPermission(
                IdentityServerPermissions.ApiScopes.Default,
                L($"Permission:{IdentityServerPermissions.ApiScopes.Default}"));

            apiScopePermission.AddChild(
                IdentityServerPermissions.ApiScopes.Create,
                L($"Permission:{IdentityServerPermissions.ApiScopes.Create}"));

            apiScopePermission.AddChild(
                IdentityServerPermissions.ApiScopes.Update,
                L($"Permission:{IdentityServerPermissions.ApiScopes.Update}"));

            apiScopePermission.AddChild(
                IdentityServerPermissions.ApiScopes.Delete,
                L($"Permission:{IdentityServerPermissions.ApiScopes.Delete}"));

            //clients
            var clientPermission = identityServerGroup.AddPermission(
                IdentityServerPermissions.Clients.Default,
                L($"Permission:{IdentityServerPermissions.Clients.Default}"));

            clientPermission.AddChild(
                IdentityServerPermissions.Clients.Create,
                L($"Permission:{IdentityServerPermissions.Clients.Create}"));

            clientPermission.AddChild(
                IdentityServerPermissions.Clients.Update,
                L($"Permission:{IdentityServerPermissions.Clients.Update}"));

            clientPermission.AddChild(
                IdentityServerPermissions.Clients.Delete,
                L($"Permission:{IdentityServerPermissions.Clients.Delete}"));

        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<IdentityServerResource>(name);
        }
    }
}