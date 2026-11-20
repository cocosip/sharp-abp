using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OceanBaseConnector;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.EntityFrameworkCore.ConnectionStrings
{
    [Dependency(ReplaceServices = true)]
    public class OceanBaseConnectionStringChecker : IConnectionStringChecker, ITransientDependency
    {
        protected ILogger Logger { get; }

        public OceanBaseConnectionStringChecker(ILogger<OceanBaseConnectionStringChecker> logger)
        {
            Logger = Check.NotNull(logger, nameof(logger));
        }

        public virtual async Task<AbpConnectionStringCheckResult> CheckAsync(string connectionString)
        {
            Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));
            var result = new AbpConnectionStringCheckResult();
            try
            {
                var builder = new OceanBaseConnectionStringBuilder(connectionString) { ConnectionTimeout = 1 };
                await using var connection = new OceanBaseConnection(builder.ConnectionString);
                await connection.OpenAsync();
                result.Connected = true;
                result.DatabaseExists = connection.CompatibilityMode == OceanBaseCompatibilityMode.Oracle
                    || !string.IsNullOrWhiteSpace(connection.Database);
            }
            catch (Exception exception)
            {
                // Driver messages can contain connection string values, so log only the failure type.
                Logger.LogWarning("OceanBase connection check failed ({ErrorType}).", exception.GetType().Name);
            }

            return result;
        }
    }
}
