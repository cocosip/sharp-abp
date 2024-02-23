using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.CryptoVault.MongoDB
{
    public static class CryptoVaultMongoDbContextExtensions
    {
        public static void ConfigureCryptoVault(
            this IMongoModelBuilder builder,
            Action<AbpMongoModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new AbpCryptoVaultMongoModelBuilderConfigurationOptions(
                AbpCryptoVaultDbProperties.DbTablePrefix
            );

            optionsAction?.Invoke(options);

            builder.Entity<RSACreds>(b =>
            {
                b.CollectionName = options.CollectionPrefix + "RSACreds";
            });

            builder.Entity<SM2Creds>(b =>
            {
                b.CollectionName = options.CollectionPrefix + "SM2Creds";
            });
        }
    }
}
