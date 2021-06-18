using JetBrains.Annotations;
using System.Data;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.DbConnections
{
    public class DefaultDbConnectionFactory : IDbConnectionFactory, ITransientDependency
    {
        protected IDbConnectionInfoResolver DbConnectionInfoResolver { get; }
        protected IDbConnectionCreateService DbConnectionCreateService { get; }
        public DefaultDbConnectionFactory(
            IDbConnectionInfoResolver dbConnectionInfoResolver,
            IDbConnectionCreateService dbConnectionCreateService)
        {
            DbConnectionInfoResolver = dbConnectionInfoResolver;
            DbConnectionCreateService = dbConnectionCreateService;
        }

        /// <summary>
        /// Get DbConnectionInfo
        /// </summary>
        /// <param name="dbConnectionName"></param>
        /// <returns></returns>
        [NotNull]
        public virtual async Task<DbConnectionInfo> GetDbConnectionInfoAsync([NotNull] string dbConnectionName)
        {
            Check.NotNullOrWhiteSpace(dbConnectionName, nameof(dbConnectionName));
            return await DbConnectionInfoResolver.ResolveAsync(dbConnectionName);
        }

        /// <summary>
        /// Get DbConnectionInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual async Task<DbConnectionInfo> GetDbConnectionInfoAsync<T>()
        {
            var dbConnectionName = DbConnectionNameAttribute.GetDbConnectionName<T>();
            return await GetDbConnectionInfoAsync(dbConnectionName);
        }

        /// <summary>
        /// Get DbConnection
        /// </summary>
        /// <param name="dbConnectionName"></param>
        /// <returns></returns>
        [NotNull]
        public virtual async Task<IDbConnection> GetDbConnectionAsync([NotNull] string dbConnectionName)
        {
            Check.NotNullOrWhiteSpace(dbConnectionName, nameof(dbConnectionName));
            return await DbConnectionCreateService.CreateAsync(dbConnectionName);
        }

        /// <summary>
        /// Get DbConnection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual async Task<IDbConnection> GetDbConnectionAsync<T>()
        {
            var dbConnectionName = DbConnectionNameAttribute.GetDbConnectionName<T>();
            return await GetDbConnectionAsync(dbConnectionName);
        }
    }
}
