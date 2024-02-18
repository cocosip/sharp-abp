using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.CryptoVault.EntityFrameworkCore
{
    [ConnectionStringName(AbpCryptoVaultDbProperties.ConnectionStringName)]
    public interface IAbpCryptoVaultDbContext : IEfCoreDbContext
    {
        DbSet<RSACreds> RSACreds { get; set; }

        DbSet<SM2Creds> SM2Creds { get; set; }
    }
}
