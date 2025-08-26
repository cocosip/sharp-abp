using Dm;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.EntityFrameworkCore.ConnectionStrings
{
    /// <summary>
    /// "DM database connection string checker implementation"
    /// </summary>
    [Dependency(ReplaceServices = true)]
    public class DmConnectionStringChecker : IConnectionStringChecker, ITransientDependency
    {
        /// <summary>
        /// "Logger instance for recording connection check activities"
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// "Initializes a new instance of the DmConnectionStringChecker class"
        /// </summary>
        /// <param name="logger">"Logger instance for recording activities"</param>
        /// <exception cref="ArgumentNullException">"Thrown when logger is null"</exception>
        public DmConnectionStringChecker(ILogger<DmConnectionStringChecker> logger)
        {
            Logger = Check.NotNull(logger, nameof(logger));
        }


        /// <summary>
        /// "Checks the DM database connection string asynchronously"
        /// </summary>
        /// <param name="connectionString">"The connection string to validate"</param>
        /// <returns>"A task that represents the asynchronous operation. The task result contains the connection check result"</returns>
        /// <exception cref="ArgumentException">"Thrown when connectionString is null or empty"</exception>
        public virtual async Task<AbpConnectionStringCheckResult> CheckAsync(string connectionString)
        {
            Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            var result = new AbpConnectionStringCheckResult();
            
            Logger.LogDebug("Starting DM database connection check");

            try
            {
                var connString = new DmConnectionStringBuilder(connectionString)
                {
                    ConnectionTimeout = 1
                };

                await using var conn = new DmConnection(connString.ConnectionString);
                await conn.OpenAsync();
                
                result.Connected = true;
                result.DatabaseExists = true;
                
                Logger.LogInformation("Successfully connected to DM database");
                
                await conn.CloseAsync();
                return result;
            }
            catch (ArgumentException ex)
            {
                Logger.LogError(ex, "Invalid DM connection string format: {ErrorMessage}", ex.Message);
                return result;
            }
            catch (DmException ex)
            {
                Logger.LogError(ex, "DM database connection failed - Error Code: {ErrorCode}, Message: {ErrorMessage}", 
                    ex.ErrorCode, ex.Message);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while checking DM database connection: {ErrorMessage}", 
                    ex.Message);
                return result;
            }
        }
    }
}
