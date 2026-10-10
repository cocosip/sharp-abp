using System;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public class ConcurrencyLimitingContextException : Exception
    {
        public ConcurrencyLimitingContextException(string message)
            : base(message)
        {
        }
    }

}
