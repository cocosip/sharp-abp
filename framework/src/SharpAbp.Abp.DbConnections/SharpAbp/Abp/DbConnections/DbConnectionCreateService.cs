using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnections
{
    public class DbConnectionCreateService : IDbConnectionCreateService, ITransientDependency
    {
        protected AbpDbConnectionsOptions Options { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected IDbConnectionResolver DbConnectionResolver { get; }
        public DbConnectionCreateService(
            IOptions<AbpDbConnectionsOptions> options,
            IServiceProvider serviceProvider,
            IDbConnectionResolver dbConnectionResolver)
        {
            Options = options.Value;
            ServiceProvider = serviceProvider;
            DbConnectionResolver = dbConnectionResolver;
        }

        /// <summary>
        /// Create dbConnection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual async Task<IDbConnection> CreateDbConnectionAsync([NotNull] string name)
        {
            var dbConnectionInfo = await DbConnectionResolver.ResolveAsync(name);
            if (dbConnectionInfo == null)
            {
                throw new AbpException($"Could not find DbConnectionInfo by name '{name}'. ");
            }

            var providerConfiguration = Options.DatabaseProviders.GetConfiguration(dbConnectionInfo.DatabaseProvider);
            if (providerConfiguration == null)
            {
                throw new AbpException($"Database '{dbConnectionInfo.DatabaseProvider}' not support.");
            }

            using var scope = ServiceProvider.CreateScope();
            foreach (var creatorType in providerConfiguration.Creators)
            {
                var dbConnectionCreator = scope.ServiceProvider
                    .GetRequiredService(creatorType)
                    .As<IDbConnectionCreator>();

                var dbConnection = await dbConnectionCreator.CreateDbConnectionAsync(name, dbConnectionInfo);
                return dbConnection;
            }

            var internalCreators = scope.ServiceProvider.GetServices<IDbConnectionInternalCreator>();
            foreach (var internalCreator in internalCreators)
            {
                if (internalCreator.DatabaseProvider == dbConnectionInfo.DatabaseProvider)
                {
                    return internalCreator.Create(dbConnectionInfo);
                }
            }

            throw new AbpException("Could not create dbConnection ,because ");
        }

    }
}
