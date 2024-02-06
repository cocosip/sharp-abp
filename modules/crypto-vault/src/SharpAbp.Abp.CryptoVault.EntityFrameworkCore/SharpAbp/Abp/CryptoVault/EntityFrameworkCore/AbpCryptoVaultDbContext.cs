using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.CryptoVault.EntityFrameworkCore
{
    [ConnectionStringName(AbpCryptoVaultDbProperties.ConnectionStringName)]
    public class AbpCryptoVaultDbContext : AbpDbContext<AbpCryptoVaultDbContext>, IAbpCryptoVaultDbContext
    {
        public AbpCryptoVaultDbContext(DbContextOptions<AbpCryptoVaultDbContext> options) 
            : base(options)
        {

        }
    }
}
