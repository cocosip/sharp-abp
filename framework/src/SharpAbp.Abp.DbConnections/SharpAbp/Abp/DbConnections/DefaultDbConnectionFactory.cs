using JetBrains.Annotations;
using System.Data;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections
{
    public class DefaultDbConnectionFactory : IDbConnectionFactory, ITransientDependency
    {
        protected IDbConnectionResolver DbConnectionResolver { get; }
        protected IDbConnectionCreateService DbConnectionCreateService { get; }
        public DefaultDbConnectionFactory(
            IDbConnectionResolver dbConnectionResolver,
            IDbConnectionCreateService dbConnectionCreateService)
        {
            DbConnectionResolver = dbConnectionResolver;
            DbConnectionCreateService = dbConnectionCreateService;
        }

        /// <summary>
        /// Get dbConnection info
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [NotNull]
        public virtual async Task<DbConnectionInfo> GetDbConnectionInfoAsync([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return await DbConnectionResolver.ResolveAsync(name);
        }

        /// <summary>
        /// Get dbConnection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [NotNull]
        public virtual async Task<IDbConnection> GetDbConnectionAsync([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return await DbConnectionCreateService.CreateDbConnectionAsync(name);
        }

    }
}
