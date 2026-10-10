

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingStoreResult
    {
        public bool IsAccepted { get; }

        public OperationRateLimitingStoreState State { get; }

        public OperationRateLimitingStoreResult(bool isAccepted, OperationRateLimitingStoreState state)
        {
            IsAccepted = isAccepted;
            State = state;
        }
    }
}
