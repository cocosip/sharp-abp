using Dm;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.EntityFrameworkCore.ConnectionStrings
{
    [Dependency(ReplaceServices = true)]
    public class DmConnectionStringChecker : IConnectionStringChecker, ITransientDependency
    {
        public virtual async Task<AbpConnectionStringCheckResult> CheckAsync(string connectionString)
        {
            var result = new AbpConnectionStringCheckResult();
            var connString = new DmConnectionStringBuilder(connectionString)
            {
                ConnectionTimeout = 1
            };

            try
            {
                await using var conn = new DmConnection(connString.ConnectionString);
                await conn.OpenAsync();
                result.Connected = true;
                result.DatabaseExists = true;

                await conn.CloseAsync();

                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
    }
}
