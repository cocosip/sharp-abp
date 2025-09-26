using JetBrains.Annotations;
using System.Data;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections
{
    /// <summary>
    /// Default implementation of IDbConnectionFactory that creates and manages database connections
    /// </summary>
    public class DefaultDbConnectionFactory : IDbConnectionFactory, ITransientDependency
    {
        /// <summary>
        /// Gets the database connection information resolver
        /// </summary>
        protected IDbConnectionInfoResolver DbConnectionInfoResolver { get; }

        /// <summary>
        /// Gets the database connection creation service
        /// </summary>
        protected IDbConnectionCreateService DbConnectionCreateService { get; }

        /// <summary>
        /// Initializes a new instance of the DefaultDbConnectionFactory class
        /// </summary>
        /// <param name="dbConnectionInfoResolver">The database connection information resolver</param>
        /// <param name="dbConnectionCreateService">The database connection creation service</param>
        public DefaultDbConnectionFactory(
            IDbConnectionInfoResolver dbConnectionInfoResolver,
            IDbConnectionCreateService dbConnectionCreateService)
        {
            DbConnectionInfoResolver = dbConnectionInfoResolver;
            DbConnectionCreateService = dbConnectionCreateService;
        }

        /// <summary>
        /// Gets the database connection information for the specified connection name
        /// </summary>
        /// <param name="dbConnectionName">The name of the database connection</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information.</returns>
        [NotNull]
        public virtual async Task<DbConnectionInfo?> GetDbConnectionInfoAsync([NotNull] string dbConnectionName)
        {
            Check.NotNullOrWhiteSpace(dbConnectionName, nameof(dbConnectionName));
            return Check.NotNull(await DbConnectionInfoResolver.ResolveAsync(dbConnectionName), nameof(DbConnectionInfo));
        }

        /// <summary>
        /// Gets the database connection information for the specified connection type
        /// </summary>
        /// <typeparam name="T">The type of the database connection</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information.</returns>
        public virtual async Task<DbConnectionInfo?> GetDbConnectionInfoAsync<T>()
        {
            var dbConnectionName = DbConnectionNameAttribute.GetDbConnectionName<T>();
            return await GetDbConnectionInfoAsync(dbConnectionName);
        }

        /// <summary>
        /// Gets a database connection for the specified connection name
        /// </summary>
        /// <param name="dbConnectionName">The name of the database connection</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection.</returns>
        [NotNull]
        public virtual async Task<IDbConnection?> GetDbConnectionAsync([NotNull] string dbConnectionName)
        {
            Check.NotNullOrWhiteSpace(dbConnectionName, nameof(dbConnectionName));
            return Check.NotNull(await DbConnectionCreateService.CreateAsync(dbConnectionName), nameof(IDbConnection));
        }

        /// <summary>
        /// Gets a database connection for the specified connection type
        /// </summary>
        /// <typeparam name="T">The type of the database connection</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection.</returns>
        public virtual async Task<IDbConnection?> GetDbConnectionAsync<T>()
        {
            var dbConnectionName = DbConnectionNameAttribute.GetDbConnectionName<T>();
            return await GetDbConnectionAsync(dbConnectionName);
        }
    }
}