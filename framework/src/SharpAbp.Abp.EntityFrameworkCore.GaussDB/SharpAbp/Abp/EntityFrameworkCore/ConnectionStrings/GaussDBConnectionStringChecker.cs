using System;
using System.Threading.Tasks;
using GaussDB;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.EntityFrameworkCore.ConnectionStrings
{
    [Dependency(ReplaceServices = true)]
    public class GaussDBConnectionStringChecker : IConnectionStringChecker, ITransientDependency
    {
        protected ILogger Logger { get; }
        public GaussDBConnectionStringChecker(ILogger<GaussDBConnectionStringChecker> logger)
        {
            Logger = logger;
        }


        public virtual async Task<AbpConnectionStringCheckResult> CheckAsync(string connectionString)
        {
            var result = new AbpConnectionStringCheckResult();
            var connString = new GaussDBConnectionStringBuilder(connectionString)
            {
                Timeout = 1
            };

            try
            {
                await using var conn = new GaussDBConnection(connString.ConnectionString);
                await conn.OpenAsync();
                result.Connected = true;
                result.DatabaseExists = true;

                await conn.CloseAsync();

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "check GaussDB database error -> {Message}", ex.Message);
                return result;
            }
        }
    }
}
