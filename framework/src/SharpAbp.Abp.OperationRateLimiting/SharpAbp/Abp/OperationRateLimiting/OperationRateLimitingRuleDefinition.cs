using System;
using System.Collections.Generic;
using System.Globalization;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingRuleDefinition
    {
        public string Name { get; }
        public bool IsNamed { get; }
        public Type RuleType { get; }
        public TimeSpan Duration { get; }
        public int MaxCount { get; }
        public string Partition { get; }
        public IReadOnlyList<OperationRateLimitingPartition> Partitions { get; }
        public bool MultiTenancy { get; }

        public OperationRateLimitingRuleDefinition(string? name, Type ruleType, TimeSpan duration,
            int maxCount, string partition = "Global", bool multiTenancy = false)
            : this(name, ruleType, duration, maxCount,
                new[] { OperationRateLimitingPartition.FromLegacyName(partition) }, multiTenancy)
        {
        }

        public OperationRateLimitingRuleDefinition(string? name, Type ruleType, TimeSpan duration,
            int maxCount, IEnumerable<OperationRateLimitingPartition> partitions, bool multiTenancy = false)
        {
            if (!typeof(IOperationRateLimitingRule).IsAssignableFrom(ruleType) || ruleType.IsAbstract || ruleType.IsInterface)
            {
                throw new OperationRateLimitingConfigurationException("RuleType must implement IOperationRateLimitingRule.");
            }

            if (ruleType == typeof(FixedWindowOperationRateLimitingRule) && (duration <= TimeSpan.Zero || maxCount < 0))
            {
                throw new OperationRateLimitingConfigurationException("Fixed window duration must be positive and maxCount nonnegative.");
            }

            if (partitions == null || (name != null && string.IsNullOrWhiteSpace(name)))
            {
                throw new OperationRateLimitingConfigurationException("Rule and partition names must be nonempty.");
            }

            var partitionList = new List<OperationRateLimitingPartition>(partitions);
            if (partitionList.Count == 0 || partitionList.Exists(partition => partition == null))
            {
                throw new OperationRateLimitingConfigurationException("A rule must contain at least one partition.");
            }

            var partitionNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (var partition in partitionList)
            {
                if (!partitionNames.Add(partition.Name))
                {
                    throw new OperationRateLimitingConfigurationException("Partition names must be unique within a rule.");
                }
            }

            IsNamed = name != null;
            var identityComponents = new List<string>
            {
                ruleType.FullName ?? ruleType.Name,
                duration.Ticks.ToString(CultureInfo.InvariantCulture),
                maxCount.ToString(CultureInfo.InvariantCulture)
            };
            if (partitionList.Count == 1)
            {
                // Keep the unnamed single-partition identity compatible with the original API.
                identityComponents.Add(partitionList[0].Name);
                identityComponents.Add(multiTenancy.ToString());
            }
            else
            {
                identityComponents.Add(multiTenancy.ToString());
                foreach (var partition in partitionList)
                {
                    identityComponents.Add(partition.GetIdentity());
                }
            }

            Name = name ?? OperationRateLimitingKeyBuilder.HashComponents(identityComponents.ToArray());
            RuleType = ruleType;
            Duration = duration;
            MaxCount = maxCount;
            Partitions = partitionList.AsReadOnly();
            Partition = partitionList[0].Name;
            MultiTenancy = multiTenancy;
        }
    }
}
