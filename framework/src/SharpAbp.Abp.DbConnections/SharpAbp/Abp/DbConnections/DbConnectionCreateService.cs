using System;
using System.Data;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections
{
    /// <summary>
    /// Service for creating database connections based on connection information
    /// </summary>
    public class DbConnectionCreateService : IDbConnectionCreateService, ITransientDependency
    {
        /// <summary>
        /// Gets the database connections configuration options
        /// </summary>
        protected AbpDbConnectionsOptions Options { get; }

        /// <summary>
        /// Gets the service provider for resolving dependencies
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the database connection information resolver
        /// </summary>
        protected IDbConnectionInfoResolver DbConnectionInfoResolver { get; }

        /// <summary>
        /// Initializes a new instance of the DbConnectionCreateService class
        /// </summary>
        /// <param name="options">The database connections configuration options</param>
        /// <param name="serviceProvider">The service provider for resolving dependencies</param>
        /// <param name="dbConnectionInfoResolver">The database connection information resolver</param>
        public DbConnectionCreateService(
            IOptions<AbpDbConnectionsOptions> options,
            IServiceProvider serviceProvider,
            IDbConnectionInfoResolver dbConnectionInfoResolver)
        {
            Options = options.Value;
            ServiceProvider = serviceProvider;
            DbConnectionInfoResolver = dbConnectionInfoResolver;
        }

        /// <summary>
        /// Creates a database connection for the specified connection name
        /// </summary>
        /// <param name="dbConnectionName">The name of the database connection to create</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created database connection.</returns>
        /// <exception cref="Volo.Abp.AbpException">Thrown when no database connection information is found for the specified name or when the database provider is not supported</exception>
        [NotNull]
        public virtual async Task<IDbConnection?> CreateAsync([NotNull] string dbConnectionName)
        {
            Check.NotNullOrWhiteSpace(dbConnectionName, nameof(dbConnectionName));
            var dbConnectionInfo = await DbConnectionInfoResolver.ResolveAsync(dbConnectionName)
                ?? throw new AbpException($"No database connection information found for connection name '{dbConnectionName}'. Please check your configuration.");

            if (!Options.DatabaseProviders.Contains(dbConnectionInfo.DatabaseProvider))
            {
                throw new AbpException($"Database provider '{dbConnectionInfo.DatabaseProvider}' is not supported. Please check your configuration.");
            }

            using var scope = ServiceProvider.CreateScope();
            var creators = scope.ServiceProvider.GetServices<IDbConnectionCreator>();
            if (creators != null)
            {
                foreach (var creator in creators)
                {
                    if (creator.IsMatch(dbConnectionName, dbConnectionInfo))
                    {
                        return await creator.CreateAsync(dbConnectionName, dbConnectionInfo);
                    }
                }
            }

            //InternalDbConnectionCreator
            var internalCreator = ServiceProvider.GetRequiredKeyedService<IInternalDbConnectionCreator>(dbConnectionInfo.DatabaseProvider);
            return internalCreator.Create(dbConnectionInfo);
        }

        /// <summary>
        /// Creates a database connection using the specified connection information
        /// </summary>
        /// <param name="dbConnectionInfo">The database connection information</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created database connection.</returns>
        /// <exception cref="Volo.Abp.AbpException">Thrown when the database provider is not supported</exception>
        [NotNull]
        public virtual Task<IDbConnection?> CreateAsync([NotNull] DbConnectionInfo dbConnectionInfo)
        {
            Check.NotNull(dbConnectionInfo, nameof(DbConnectionInfo));
            if (!Options.DatabaseProviders.Contains(dbConnectionInfo.DatabaseProvider))
            {
                throw new AbpException($"Database provider '{dbConnectionInfo.DatabaseProvider}' is not supported. Please check your configuration.");
            }
            var internalCreator = ServiceProvider.GetKeyedService<IInternalDbConnectionCreator>(dbConnectionInfo.DatabaseProvider);
            return Task.FromResult(internalCreator?.Create(dbConnectionInfo));
        }
    }
}