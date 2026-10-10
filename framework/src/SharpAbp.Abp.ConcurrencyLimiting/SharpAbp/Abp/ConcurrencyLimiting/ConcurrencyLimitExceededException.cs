using Volo.Abp;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public enum ConcurrencyLimitExceededReason
    {
        Capacity, QueueFull, QueueTimeout
    }
    /// <summary>Admission failed due to execution capacity, bounded queue capacity, or queue timeout.</summary>
    public class ConcurrencyLimitExceededException : BusinessException
    {
        public string PolicyName { get; }
        public int PermitLimit { get; }
        public int QueueLimit { get; }
        public ConcurrencyLimitExceededReason Reason { get; }
        public ConcurrencyLimitExceededException(ConcurrencyLimitingPolicy policy, ConcurrencyLimitExceededReason reason)
            : base(policy.ErrorCode)
        {
            PolicyName = policy.Name;
            PermitLimit = policy.PermitLimit;
            QueueLimit = policy.QueueLimit;
            Reason = reason;
            WithData(nameof(PolicyName), PolicyName);
            WithData(nameof(PermitLimit), PermitLimit);
            WithData(nameof(QueueLimit), QueueLimit);
            WithData(nameof(Reason), Reason.ToString());
        }
    }

}
