

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingRuleResult
    {
        public bool IsAccepted { get; }
        public OperationRateLimitingRuleStatus Status { get; }

        public OperationRateLimitingRuleResult(bool isAccepted, OperationRateLimitingRuleStatus status)
        {
            IsAccepted = isAccepted;
            Status = status;
        }
    }
}
