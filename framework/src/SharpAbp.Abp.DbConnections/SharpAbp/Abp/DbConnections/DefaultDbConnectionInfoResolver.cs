using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections
{
    public class DefaultDbConnectionInfoResolver : IDbConnectionInfoResolver, ITransientDependency
    {
        protected AbpDbConnectionsOptions Options { get; }
        public DefaultDbConnectionInfoResolver(IOptions<AbpDbConnectionsOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Resolve DbConnectionInfo
        /// </summary>
        /// <param name="dbConnectionName"></param>
        /// <returns></returns>
        public virtual async Task<DbConnectionInfo> ResolveAsync([NotNull] string dbConnectionName)
        {
            Check.NotNullOrWhiteSpace(dbConnectionName, nameof(dbConnectionName));
            var dbConnectionConfiguration = Options.DbConnections.GetConfiguration(dbConnectionName);
            if (dbConnectionConfiguration == null)
            {
                throw new AbpException($"Could not find DbConnection by dbConnectionName '{dbConnectionName}'.");
            }

            var dbConnectionInfo = new DbConnectionInfo(
                dbConnectionConfiguration.DatabaseProvider,
                dbConnectionConfiguration.ConnectionString
                );

            return await Task.FromResult(dbConnectionInfo);
        }
    }
}
