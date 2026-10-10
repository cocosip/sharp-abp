using Volo.Abp;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingConfigurationException : AbpException
    {
        public OperationRateLimitingConfigurationException(string message) : base(message)
        {
        }
    }
}
