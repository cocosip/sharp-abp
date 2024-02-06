using Volo.Abp.Data;

namespace SharpAbp.Abp.CryptoVault
{
    public class AbpCryptoVaultDbProperties
    {
        public static string DbTablePrefix { get; set; } = AbpCommonDbProperties.DbTablePrefix;

        public static string DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

        public const string ConnectionStringName = "AbpCryptoVault";
    }
}
