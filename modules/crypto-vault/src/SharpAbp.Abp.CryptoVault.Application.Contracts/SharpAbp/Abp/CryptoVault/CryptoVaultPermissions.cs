using Volo.Abp.Reflection;

namespace SharpAbp.Abp.CryptoVault
{
    public class CryptoVaultPermissions
    {
        public const string GroupName = "AbpCryptoVault";

        public static class RSACreds
        {
            public const string Default = GroupName + ".RSACreds";
            public const string Generate = Default + ".Generate";
            public const string Import = Default + ".Import";
            public const string DecryptKey = Default + ".DecryptKey";
            public const string Delete = Default + ".Delete";
        }

        public static class SM2Creds
        {
            public const string Default = GroupName + ".SM2Creds";
            public const string Generate = Default + ".Generate";
            public const string Import = Default + ".Import";
            public const string DecryptKey = Default + ".DecryptKey";
            public const string Delete = Default + ".Delete";
        }

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(CryptoVaultPermissions));
        }
    }
}
