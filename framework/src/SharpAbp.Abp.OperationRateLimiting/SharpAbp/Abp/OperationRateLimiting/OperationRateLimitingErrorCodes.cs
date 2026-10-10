

namespace SharpAbp.Abp.OperationRateLimiting
{
    public static class OperationRateLimitingErrorCodes
    {
        public const string LimitExceeded = "OperationRateLimiting:LimitExceeded";
        public const string PermanentlyDenied = "OperationRateLimiting:PermanentlyDenied";
    }
}
