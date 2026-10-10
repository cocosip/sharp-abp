using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingRuleBuilder
    {
        protected string? Name { get; set; }
        protected Type RuleType { get; set; } = typeof(FixedWindowOperationRateLimitingRule);
        protected TimeSpan Duration { get; set; }
        protected int MaxCount { get; set; }
        protected IList<OperationRateLimitingPartition> Partitions { get; } =
            new List<OperationRateLimitingPartition> { OperationRateLimitingPartition.Global() };
        protected bool MultiTenancy { get; set; }

        public OperationRateLimitingRuleBuilder()
        {
        }

        public OperationRateLimitingRuleBuilder(OperationRateLimitingRuleDefinition definition)
        {
            Name = definition.IsNamed ? definition.Name : null;
            RuleType = definition.RuleType;
            Duration = definition.Duration;
            MaxCount = definition.MaxCount;
            Partitions.Clear();
            foreach (var partition in definition.Partitions)
            {
                Partitions.Add(partition);
            }
            MultiTenancy = definition.MultiTenancy;
        }

        public virtual OperationRateLimitingRuleBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        public virtual OperationRateLimitingRuleBuilder WithFixedWindow(TimeSpan duration, int maxCount)
        {
            RuleType = typeof(FixedWindowOperationRateLimitingRule);
            Duration = duration;
            MaxCount = maxCount;
            return this;
        }

        public virtual OperationRateLimitingRuleBuilder WithRule<TRule>() where TRule : IOperationRateLimitingRule
        {
            RuleType = typeof(TRule);
            return this;
        }

        public virtual OperationRateLimitingRuleBuilder WithMultiTenancy(bool enabled = true)
        {
            MultiTenancy = enabled;
            return this;
        }

        public virtual OperationRateLimitingRuleBuilder PartitionGlobally()
        {
            return PartitionBy(OperationRateLimitingPartition.Global());
        }

        public virtual OperationRateLimitingRuleBuilder PartitionByParameter()
        {
            return PartitionBy(OperationRateLimitingPartition.Parameter());
        }

        public virtual OperationRateLimitingRuleBuilder PartitionByCurrentUser()
        {
            return PartitionBy(OperationRateLimitingPartition.CurrentUser());
        }

        public virtual OperationRateLimitingRuleBuilder PartitionByCurrentTenant()
        {
            return PartitionBy(OperationRateLimitingPartition.CurrentTenant());
        }

        public virtual OperationRateLimitingRuleBuilder PartitionByEmail()
        {
            return PartitionBy(OperationRateLimitingPartition.Email());
        }

        public virtual OperationRateLimitingRuleBuilder PartitionByPhoneNumber()
        {
            return PartitionBy(OperationRateLimitingPartition.PhoneNumber());
        }

        public virtual OperationRateLimitingRuleBuilder PartitionBy(string resolverName)
        {
            return PartitionBy(OperationRateLimitingPartition.FromLegacyName(resolverName));
        }

        public virtual OperationRateLimitingRuleBuilder PartitionBy(
            params OperationRateLimitingPartition[] partitions)
        {
            if (partitions == null || partitions.Length == 0)
            {
                throw new OperationRateLimitingConfigurationException("A rule must contain at least one partition.");
            }

            Partitions.Clear();
            foreach (var partition in partitions)
            {
                if (partition == null)
                {
                    throw new OperationRateLimitingConfigurationException("Partition cannot be null.");
                }

                Partitions.Add(partition);
            }

            return this;
        }

        public virtual OperationRateLimitingRuleDefinition Build()
        {
            return new OperationRateLimitingRuleDefinition(Name, RuleType, Duration, MaxCount, Partitions, MultiTenancy);
        }
    }
}
