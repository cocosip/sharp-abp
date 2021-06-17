using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections
{
    public class DefaultDbConnectionResolver : IDbConnectionResolver, ITransientDependency
    {
        protected AbpDbConnectionsOptions Options { get; }
        public DefaultDbConnectionResolver(IOptions<AbpDbConnectionsOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Resolve by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [NotNull]
        public virtual async Task<DbConnectionInfo> ResolveAsync([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            var configuration = Options.DbConnections.GetConfiguration(name);
            if (configuration == null)
            {
                throw new AbpException($"Could not find dbConnection by name '{name}'.");
            }

            var dbConnectionInfo = new DbConnectionInfo()
            {
                DatabaseProvider = configuration.DatabaseProvider,
                ConnectionString = configuration.ConnectionString
            };

            return await Task.FromResult(dbConnectionInfo);
        }

    }
}
