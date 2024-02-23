using SharpAbp.Abp.CryptoVault.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace SharpAbp.Abp.CryptoVault
{
    public class AbpCryptoVaultPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var cryptoVaultGroup = context.AddGroup(CryptoVaultPermissions.GroupName, L(CryptoVaultPermissions.GroupName));

            var rsaCredsPermission = cryptoVaultGroup.AddPermission(
                CryptoVaultPermissions.RSACreds.Default,
                L($"Permission:{CryptoVaultPermissions.RSACreds.Default}"));

            rsaCredsPermission.AddChild(
                CryptoVaultPermissions.RSACreds.Generate,
                L($"Permission:{CryptoVaultPermissions.RSACreds.Generate}"));

            rsaCredsPermission.AddChild(
                CryptoVaultPermissions.RSACreds.Create,
                L($"Permission:{CryptoVaultPermissions.RSACreds.Create}"));

            rsaCredsPermission.AddChild(
                CryptoVaultPermissions.RSACreds.DecryptKey,
                L($"Permission:{CryptoVaultPermissions.RSACreds.DecryptKey}"));

            rsaCredsPermission.AddChild(
                CryptoVaultPermissions.RSACreds.Delete,
                L($"Permission:{CryptoVaultPermissions.RSACreds.Delete}"));

            var sm2CredsPermission = cryptoVaultGroup.AddPermission(
                CryptoVaultPermissions.SM2Creds.Default,
                L($"Permission:{CryptoVaultPermissions.SM2Creds.Default}"));

            sm2CredsPermission.AddChild(
                CryptoVaultPermissions.SM2Creds.Generate,
                L($"Permission:{CryptoVaultPermissions.SM2Creds.Generate}"));

            sm2CredsPermission.AddChild(
                CryptoVaultPermissions.SM2Creds.Create,
                L($"Permission:{CryptoVaultPermissions.SM2Creds.Create}"));

            sm2CredsPermission.AddChild(
               CryptoVaultPermissions.SM2Creds.DecryptKey,
               L($"Permission:{CryptoVaultPermissions.SM2Creds.DecryptKey}"));

            sm2CredsPermission.AddChild(
                CryptoVaultPermissions.SM2Creds.Delete,
                L($"Permission:{CryptoVaultPermissions.SM2Creds.Delete}"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AbpCryptoVaultResource>(name);
        }
    }
}
