using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections
{
    /// <summary>
    /// Default implementation of IDbConnectionInfoResolver that resolves database connection information from configuration
    /// </summary>
    public class DefaultDbConnectionInfoResolver : IDbConnectionInfoResolver, ITransientDependency
    {
        /// <summary>
        /// Gets the database connections configuration options
        /// </summary>
        protected AbpDbConnectionsOptions Options { get; }

        /// <summary>
        /// Initializes a new instance of the DefaultDbConnectionInfoResolver class
        /// </summary>
        /// <param name="options">The database connections configuration options</param>
        public DefaultDbConnectionInfoResolver(IOptions<AbpDbConnectionsOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Resolves database connection information for the specified connection name
        /// </summary>
        /// <param name="dbConnectionName">The name of the database connection to resolve</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information.</returns>
        /// <exception cref="Volo.Abp.AbpException">Thrown when no database connection configuration is found for the specified name</exception>
        public virtual async Task<DbConnectionInfo> ResolveAsync([NotNull] string dbConnectionName)
        {
            Check.NotNullOrWhiteSpace(dbConnectionName, nameof(dbConnectionName));
            var dbConnectionConfiguration = Options.DbConnections.GetConfiguration(dbConnectionName) ?? throw new AbpException($"No database connection configuration found for connection name '{dbConnectionName}'. Please check your configuration.");
            var dbConnectionInfo = new DbConnectionInfo(
                dbConnectionConfiguration.DatabaseProvider,
                dbConnectionConfiguration.ConnectionString);

            return await Task.FromResult(dbConnectionInfo);
        }
    }
}