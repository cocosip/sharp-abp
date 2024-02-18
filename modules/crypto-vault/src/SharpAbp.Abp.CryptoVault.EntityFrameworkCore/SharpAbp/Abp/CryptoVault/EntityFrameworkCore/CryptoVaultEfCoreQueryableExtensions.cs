using System.Linq;

namespace SharpAbp.Abp.CryptoVault.EntityFrameworkCore
{
    public static class CryptoVaultEfCoreQueryableExtensions
    {
        public static IQueryable<RSACreds> IncludeDetails(
            this IQueryable<RSACreds> queryable,
            bool include = true)
        {
            return queryable;
        }

        public static IQueryable<SM2Creds> IncludeDetails(
            this IQueryable<SM2Creds> queryable,
            bool include = true)
        {
            return queryable;
        }
    }
}
