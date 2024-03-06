using SharpAbp.Abp.CryptoVault;
using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.TransformSecurityManagement.MongoDB
{
    public static class TransformSecurityManagementMongoDbContextExtensions
    {
        public static void ConfigureTransformSecurityManagement(
            this IMongoModelBuilder builder,
            Action<AbpMongoModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new AbpTransformSecurityManagementMongoModelBuilderConfigurationOptions(
                AbpCryptoVaultDbProperties.DbTablePrefix
            );

            optionsAction?.Invoke(options);

            builder.Entity<SecurityCredentialInfo>(b =>
            {
                b.CollectionName = options.CollectionPrefix + "SecurityCredentialInfos";
            });
        }
    }
}
