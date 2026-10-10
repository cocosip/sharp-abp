using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingStatus
    {
        public bool IsEnabled { get; }
        public bool IsAllowed { get; }
        public int MaxCount { get; }
        public int CurrentCount { get; }
        public int RemainingCount { get; }
        public TimeSpan? RetryAfter { get; }
        public IReadOnlyList<OperationRateLimitingRuleStatus> RuleDetails { get; }

        public OperationRateLimitingStatus(bool isEnabled, bool isAllowed, int maxCount,
            int currentCount, int remainingCount, TimeSpan? retryAfter,
            IEnumerable<OperationRateLimitingRuleStatus> ruleDetails)
        {
            IsEnabled = isEnabled;
            IsAllowed = isAllowed;
            MaxCount = maxCount;
            CurrentCount = currentCount;
            RemainingCount = remainingCount;
            RetryAfter = retryAfter;
            RuleDetails = new List<OperationRateLimitingRuleStatus>(ruleDetails).AsReadOnly();
        }
    }
}
