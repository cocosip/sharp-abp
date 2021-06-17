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
        /// Get IDbConnection
        /// </summary>
        /// <param name="dbConnectionName"></param>
        /// <returns></returns>
        [NotNull]
        public virtual async Task<IDbConnection> GetDbConnectionAsync([NotNull] string dbConnectionName)
        {
            Check.NotNullOrWhiteSpace(dbConnectionName, nameof(dbConnectionName));
            return await DbConnectionCreateService.CreateAsync(dbConnectionName);
        }
    }
}
