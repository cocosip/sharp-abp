using System;
using System.Threading.Tasks;
using GaussDB;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.EntityFrameworkCore.ConnectionStrings
{
    /// <summary>
    /// GaussDB connection string checker implementation for validating GaussDB database connections.
    /// </summary>
    [Dependency(ReplaceServices = true)]
    public class GaussDBConnectionStringChecker : IConnectionStringChecker, ITransientDependency
    {
        /// <summary>
        /// Gets the logger instance for this connection string checker.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussDBConnectionStringChecker"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging connection check operations.</param>
        public GaussDBConnectionStringChecker(ILogger<GaussDBConnectionStringChecker> logger)
        {
            Logger = Check.NotNull(logger, nameof(logger));
        }


        /// <summary>
        /// Checks the GaussDB database connection asynchronously.
        /// </summary>
        /// <param name="connectionString">The connection string to validate.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the connection check result.</returns>
        /// <exception cref="ArgumentException">Thrown when the connection string is null or empty.</exception>
        public virtual async Task<AbpConnectionStringCheckResult> CheckAsync(string connectionString)
        {
            Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            var result = new AbpConnectionStringCheckResult();
            
            Logger.LogDebug("Starting GaussDB connection validation for database connection");

            try
            {
                GaussDBConnectionStringBuilder connString;
                try
                {
                    connString = new GaussDBConnectionStringBuilder(connectionString)
                    {
                        Timeout = 1
                    };
                }
                catch (ArgumentException ex)
                {
                    Logger.LogError(ex, "Invalid GaussDB connection string format detected during validation. Connection String: {ConnectionString}", 
                        connectionString);
                    return result;
                }

                Logger.LogDebug("Attempting to establish connection to GaussDB database. Host: {Host}, Database: {Database}, Port: {Port}", 
                    connString.Host, connString.Database, connString.Port);

                await using var conn = new GaussDBConnection(connString.ConnectionString);
                await conn.OpenAsync();
                
                result.Connected = true;
                result.DatabaseExists = true;

                Logger.LogInformation("Successfully established connection to GaussDB database. Host: {Host}, Database: {Database}, Server Version: {ServerVersion}", 
                    connString.Host, connString.Database, conn.ServerVersion);

                await conn.CloseAsync();

                return result;
            }

            catch (GaussDBException ex)
            {
                Logger.LogError(ex, "GaussDB database specific error occurred during connection validation. Error Code: {ErrorCode}, SqlState: {SqlState}, Message: {Message}", 
                    ex.ErrorCode, ex.SqlState, ex.Message);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred during GaussDB connection validation. Exception Type: {ExceptionType}, Message: {Message}", 
                    ex.GetType().Name, ex.Message);
                return result;
            }
        }
    }
}
