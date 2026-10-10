using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingConfiguration : ISingletonDependency
    {
        public bool IsEnabled { get; }
        public string ApplicationName { get; }
        public TimeSpan LockTimeout { get; }
        public OperationRateLimitingCoordinationScope CoordinationScope { get; }
        public IReadOnlyDictionary<string, OperationRateLimitingPolicy> Policies { get; }
        public IReadOnlyDictionary<string, Func<IServiceProvider, OperationRateLimitingContext, CancellationToken, Task<string>>> PartitionResolvers { get; }

        public OperationRateLimitingConfiguration(IOptions<SharpAbpOperationRateLimitingOptions> options)
        {
            var value = options.Value;
            if (string.IsNullOrWhiteSpace(value.ApplicationName) || value.LockTimeout < TimeSpan.Zero ||
                !Enum.IsDefined(typeof(OperationRateLimitingCoordinationScope), value.CoordinationScope))
            {
                throw new OperationRateLimitingConfigurationException("Invalid application namespace, lock timeout or coordination scope.");
            }

            IsEnabled = value.IsEnabled;
            ApplicationName = value.ApplicationName;
            LockTimeout = value.LockTimeout;
            CoordinationScope = value.CoordinationScope;
            Policies = new System.Collections.ObjectModel.ReadOnlyDictionary<string, OperationRateLimitingPolicy>(
                new Dictionary<string, OperationRateLimitingPolicy>(value.Policies, StringComparer.Ordinal));
            PartitionResolvers = new System.Collections.ObjectModel.ReadOnlyDictionary<string, Func<IServiceProvider, OperationRateLimitingContext, CancellationToken, Task<string>>>(
                new Dictionary<string, Func<IServiceProvider, OperationRateLimitingContext, CancellationToken, Task<string>>>(value.PartitionResolvers, StringComparer.Ordinal));
            foreach (var resolver in PartitionResolvers)
            {
                if (string.IsNullOrWhiteSpace(resolver.Key) || OperationRateLimitingPartitionResolver.IsBuiltIn(resolver.Key) || resolver.Value == null)
                {
                    throw new OperationRateLimitingConfigurationException("Custom resolver names cannot replace built-in partitions.");
                }
            }
            foreach (var policy in Policies.Values)
            {
                foreach (var rule in policy.Rules)
                {
                    foreach (var partition in rule.Partitions)
                    {
                        if (partition.Kind == OperationRateLimitingPartitionKind.Resolver &&
                            !PartitionResolvers.ContainsKey(partition.ResolverName!))
                        {
                            throw new OperationRateLimitingConfigurationException(
                                "Unknown partition resolver: " + partition.ResolverName);
                        }
                    }
                }
            }
        }
    }
}
