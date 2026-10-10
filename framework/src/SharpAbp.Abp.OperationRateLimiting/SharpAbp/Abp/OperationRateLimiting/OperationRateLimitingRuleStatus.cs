using System;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingRuleStatus
    {
        public string RuleName { get; }
        public bool IsAllowed { get; }
        public int MaxCount { get; }
        public int CurrentCount { get; }
        public int RemainingCount { get; }
        public TimeSpan? RetryAfter { get; }
        public DateTime? WindowEndUtc { get; }

        public OperationRateLimitingRuleStatus(string ruleName, bool isAllowed, int maxCount,
            int currentCount, int remainingCount, TimeSpan? retryAfter, DateTime? windowEndUtc = null)
        {
            RuleName = ruleName;
            IsAllowed = isAllowed;
            MaxCount = maxCount;
            CurrentCount = currentCount;
            RemainingCount = remainingCount;
            RetryAfter = retryAfter;
            WindowEndUtc = windowEndUtc;
        }
    }
}
