using System;
using System.Collections.Generic;
using System.Globalization;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingPolicy
    {
        public string Name { get; }
        public string? ErrorCode { get; }
        public IReadOnlyList<OperationRateLimitingRuleDefinition> Rules { get; }

        public OperationRateLimitingPolicy(string name, IEnumerable<OperationRateLimitingRuleDefinition> rules,
            string? errorCode = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new OperationRateLimitingConfigurationException("Policy name must be nonempty.");
            }

            var list = new List<OperationRateLimitingRuleDefinition>(rules);
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var rule in list)
            {
                if (!names.Add(rule.Name))
                {
                    throw new OperationRateLimitingConfigurationException("Rule names must be unique within a policy.");
                }
            }

            if (list.Count == 0)
            {
                throw new OperationRateLimitingConfigurationException("A policy must contain at least one rule.");
            }

            Name = name;
            ErrorCode = errorCode;
            Rules = list.AsReadOnly();
        }
    }
}
