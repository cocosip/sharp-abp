using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.CryptoVault.EntityFrameworkCore
{
    [ConnectionStringName(AbpCryptoVaultDbProperties.ConnectionStringName)]
    public interface IAbpCryptoVaultDbContext : IEfCoreDbContext
    {
    }
}
