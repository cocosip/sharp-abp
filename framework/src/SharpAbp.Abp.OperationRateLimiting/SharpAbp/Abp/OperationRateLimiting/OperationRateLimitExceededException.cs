using Volo.Abp;
using Volo.Abp.ExceptionHandling;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitExceededException : BusinessException, IHasHttpStatusCode
    {
        public string PolicyName { get; }
        public OperationRateLimitingRuleStatus RuleStatus { get; }
        public int HttpStatusCode => 429;

        public OperationRateLimitExceededException(string policyName, OperationRateLimitingRuleStatus status,
            string? errorCode = null, string? retryDescription = null)
            : base(errorCode ?? (status.MaxCount == 0 ? OperationRateLimitingErrorCodes.PermanentlyDenied :
                OperationRateLimitingErrorCodes.LimitExceeded))
        {
            PolicyName = policyName;
            RuleStatus = status;
            WithData("PolicyName", policyName);
            WithData("RuleName", status.RuleName);
            WithData("MaxCount", status.MaxCount);
            WithData("CurrentCount", status.CurrentCount);
            WithData("RemainingCount", status.RemainingCount);
            Data["WindowEndUtc"] = status.WindowEndUtc;
            Data["RetryAfter"] = status.RetryAfter;
            Data["RetryDescription"] = retryDescription;
        }
    }
}
