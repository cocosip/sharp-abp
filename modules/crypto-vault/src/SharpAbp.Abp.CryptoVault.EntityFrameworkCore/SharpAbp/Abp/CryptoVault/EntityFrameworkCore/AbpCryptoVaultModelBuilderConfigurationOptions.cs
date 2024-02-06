using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.CryptoVault.EntityFrameworkCore
{
    public class AbpCryptoVaultModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public AbpCryptoVaultModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = "",
            [CanBeNull] string schema = null) 
            : base(tablePrefix, schema)
        {

        }
    }
}
