using SharpAbp.MinId.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace SharpAbp.MinId
{
    public class MinIdPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var minIdGroup = context.AddGroup(MinIdPermissions.GroupName, L(MinIdPermissions.GroupName));

            var minIdInfoPermission = minIdGroup.AddPermission(
                MinIdPermissions.MinIdInfos.Default,
                L($"Permission:{MinIdPermissions.MinIdInfos.Default}"));

            minIdInfoPermission.AddChild(
                MinIdPermissions.MinIdInfos.Create,
                L($"Permission:{MinIdPermissions.MinIdInfos.Create}"));

            minIdInfoPermission.AddChild(
                MinIdPermissions.MinIdInfos.Update,
                L($"Permission:{MinIdPermissions.MinIdInfos.Update}"));

            minIdInfoPermission.AddChild(
                MinIdPermissions.MinIdInfos.Delete,
                L($"Permission:{MinIdPermissions.MinIdInfos.Delete}"));

            var minIdTokenPermission = minIdGroup.AddPermission(
                MinIdPermissions.MinIdTokens.Default,
                L($"Permission:{MinIdPermissions.MinIdTokens.Default}"));

            minIdTokenPermission.AddChild(
                MinIdPermissions.MinIdTokens.Create,
                L($"Permission:{MinIdPermissions.MinIdTokens.Create}"));

            minIdTokenPermission.AddChild(
                MinIdPermissions.MinIdTokens.Update,
                L($"Permission:{MinIdPermissions.MinIdTokens.Update}"));

            minIdTokenPermission.AddChild(
                MinIdPermissions.MinIdTokens.Delete,
                L($"Permission:{MinIdPermissions.MinIdTokens.Delete}"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<MinIdResource>(name);
        }
    }
}
