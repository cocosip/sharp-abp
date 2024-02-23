using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.CryptoVault.MongoDB
{
    [ConnectionStringName(AbpCryptoVaultDbProperties.ConnectionStringName)]
    public interface IAbpCryptoVaultMongoDbContext : IAbpMongoDbContext
    {
        IMongoCollection<RSACreds> RSACreds { get; }
        IMongoCollection<SM2Creds> SM2Creds { get; }
    }
}
