using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.CryptoVault.MongoDB
{
    [ConnectionStringName(AbpCryptoVaultDbProperties.ConnectionStringName)]
    public class AbpCryptoVaultMongoDbContext : AbpMongoDbContext, IAbpCryptoVaultMongoDbContext
    {
        public IMongoCollection<RSACreds> RSACreds => Collection<RSACreds>();
        public IMongoCollection<SM2Creds> SM2Creds => Collection<SM2Creds>();
        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);
            modelBuilder.ConfigureCryptoVault();
        }
    }
}
