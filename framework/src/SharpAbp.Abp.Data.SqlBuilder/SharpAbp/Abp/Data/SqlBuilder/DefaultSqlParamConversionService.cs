using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    public class DefaultSqlParamConversionService : ISqlParamConversionService, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected DataSqlBuilderOptions Options { get; }
        protected IServiceProvider ServiceProvider { get; }
        public DefaultSqlParamConversionService(
            ILogger<DefaultSqlParamConversionService> logger,
            IOptions<DataSqlBuilderOptions> options,
            IServiceProvider serviceProvider)
        {
            Logger = logger;
            Options = options.Value;
            ServiceProvider = serviceProvider;
        }


        /// <summary>
        /// Converts the input parameter to the appropriate format for the specified database provider
        /// </summary>
        /// <param name="provider">The target database provider</param>
        /// <param name="entityType">The entity type being processed</param>
        /// <param name="inputData">The input data to be converted</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The converted parameter object</returns>
        public virtual async Task<object?> ConvertParameterAsync(
            DatabaseProvider provider,
            Type entityType,
            object? inputData,
            CancellationToken cancellationToken = default)
        {
            var ctx = new ParameterBindingContext(provider, entityType, inputData);

            using var scope = ServiceProvider.CreateScope();
            foreach (var contributorType in Options.SqlParamConversionContributors)
            {
                var contributor = scope.ServiceProvider
                    .GetRequiredService(contributorType)
                    .As<ISqlParamConversionContributor>();

                if (await contributor.IsMatchAsync(ctx, cancellationToken))
                {
                    Logger.LogDebug("SQL parameter conversion matched. Provider: {DatabaseProvider}, Type: [{EntityType}], Contributor: [{ContributorType}]",
                        provider, entityType.FullName, contributorType.Name);

                    var result = await contributor.ProcessAsync(ctx, cancellationToken);

                    // If conversion was successful, return the result
                    if (result != null)
                    {
                        Logger.LogDebug("SQL parameter conversion completed successfully. Provider: {DatabaseProvider}, Type: [{EntityType}]",
                            provider, entityType.FullName);
                        return result;
                    }
                }
                else
                {
                    Logger.LogDebug("SQL parameter conversion not matched. Provider: {DatabaseProvider}, Type: [{EntityType}], Contributor: [{ContributorType}]",
                        provider, entityType.FullName, contributorType.Name);
                }
            }

            // If no contributor processed the input, return the original data
            Logger.LogDebug("No SQL parameter conversion contributor found. Returning original data. Provider: {DatabaseProvider}, Type: [{EntityType}]",
                provider, entityType.FullName);
            return inputData;
        }

    }
}