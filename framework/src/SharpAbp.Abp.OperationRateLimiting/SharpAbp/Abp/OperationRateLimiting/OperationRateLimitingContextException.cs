using Volo.Abp;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingContextException : AbpException
    {
        public OperationRateLimitingContextException(string message) : base(message)
        {
        }
    }
}
