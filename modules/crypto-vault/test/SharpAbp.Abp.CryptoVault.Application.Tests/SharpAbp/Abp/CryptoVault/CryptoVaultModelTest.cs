using Microsoft.EntityFrameworkCore;
using SharpAbp.Abp.CryptoVault.EntityFrameworkCore;
using System;
using Xunit;

namespace SharpAbp.Abp.CryptoVault
{
    public class CryptoVaultModelTest : AbpCryptoVaultApplicationTestBase
    {
        [Fact]
        public void Key_Columns_Are_Stored_As_Lob()
        {
            var options = new DbContextOptionsBuilder<AbpCryptoVaultDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var dbContext = new AbpCryptoVaultDbContext(options);

            var rsaEntityType = dbContext.Model.FindEntityType(typeof(RSACreds));
            Assert.NotNull(rsaEntityType);
            AssertKeyColumnIsLob(rsaEntityType.FindProperty(nameof(RSACreds.PublicKey)).GetMaxLength(), RSACredsConsts.MaxPublicKeyLength);
            AssertKeyColumnIsLob(rsaEntityType.FindProperty(nameof(RSACreds.PrivateKey)).GetMaxLength(), RSACredsConsts.MaxPrivateKeyLength);

            var sm2EntityType = dbContext.Model.FindEntityType(typeof(SM2Creds));
            Assert.NotNull(sm2EntityType);
            AssertKeyColumnIsLob(sm2EntityType.FindProperty(nameof(SM2Creds.PublicKey)).GetMaxLength(), SM2CredsConsts.MaxPublicKeyLength);
            AssertKeyColumnIsLob(sm2EntityType.FindProperty(nameof(SM2Creds.PrivateKey)).GetMaxLength(), SM2CredsConsts.MaxPrivateKeyLength);

            // The short columns must stay within the Oracle NVARCHAR2 limit.
            Assert.True(rsaEntityType.FindProperty(nameof(RSACreds.Identifier)).GetMaxLength() <= 2000);
            Assert.True(rsaEntityType.FindProperty(nameof(RSACreds.PassPhrase)).GetMaxLength() <= 2000);
            Assert.True(rsaEntityType.FindProperty(nameof(RSACreds.Salt)).GetMaxLength() <= 2000);
            Assert.True(rsaEntityType.FindProperty(nameof(RSACreds.Description)).GetMaxLength() <= 2000);
            Assert.True(sm2EntityType.FindProperty(nameof(SM2Creds.Curve)).GetMaxLength() <= 2000);
        }

        private static void AssertKeyColumnIsLob(int? configuredMaxLength, int expectedMaxLength)
        {
            Assert.Equal(expectedMaxLength, configuredMaxLength);

            // Oracle maps a string column above 2000 to NCLOB. Lowering these lengths to 2000 would truncate
            // encrypted 3072/4096 bit keys, so the LOB mapping is intentional.
            Assert.True(configuredMaxLength > 2000);
        }
    }
}
