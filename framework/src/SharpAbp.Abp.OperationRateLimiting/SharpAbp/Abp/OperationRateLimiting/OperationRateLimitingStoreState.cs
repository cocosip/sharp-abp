using System;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingStoreState
    {
        public int CurrentCount { get; }

        public DateTime? WindowEndUtc { get; }

        public OperationRateLimitingStoreState(int currentCount = 0, DateTime? windowEndUtc = null)
        {
            CurrentCount = currentCount;
            WindowEndUtc = windowEndUtc;
        }
    }
}
