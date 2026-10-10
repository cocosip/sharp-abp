using System;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public interface IOperationRateLimitingFormatter
    {
        string Format(TimeSpan duration);
    }
}
