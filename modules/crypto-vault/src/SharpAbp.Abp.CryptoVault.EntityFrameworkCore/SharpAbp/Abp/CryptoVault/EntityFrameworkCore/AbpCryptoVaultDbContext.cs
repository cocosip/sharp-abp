using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.CryptoVault.EntityFrameworkCore
{
    [ConnectionStringName(AbpCryptoVaultDbProperties.ConnectionStringName)]
    public class AbpCryptoVaultDbContext : AbpDbContext<AbpCryptoVaultDbContext>, IAbpCryptoVaultDbContext
    {

        public DbSet<RSACreds> RSACreds { get; set; }
        public DbSet<SM2Creds> SM2Creds { get; set; }

        public AbpCryptoVaultDbContext(DbContextOptions<AbpCryptoVaultDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigureCryptoVault();
        }
    }
}
