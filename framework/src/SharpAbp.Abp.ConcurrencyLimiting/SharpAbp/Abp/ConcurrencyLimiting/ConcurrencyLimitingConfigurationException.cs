using System;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public class ConcurrencyLimitingConfigurationException : Exception
    {
        public ConcurrencyLimitingConfigurationException(string message)
            : base(message)
        {
        }
    }

}
