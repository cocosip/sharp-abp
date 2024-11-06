using Dm;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.EntityFrameworkCore.ConnectionStrings
{
    [Dependency(ReplaceServices = true)]
    public class DmConnectionStringChecker : IConnectionStringChecker, ITransientDependency
    {
        protected ILogger Logger { get; }
        public DmConnectionStringChecker(ILogger<DmConnectionStringChecker> logger)
        {
            Logger = logger;
        }


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
            catch (Exception ex)
            {
                Logger.LogError(ex, "check dm database error -> {Message}", ex.Message);
                return result;
            }
        }
    }
}
