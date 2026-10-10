using System;

namespace SharpAbp.Abp.OperationRateLimiting
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RateLimitingParameterAttribute : Attribute
    {
    }
}
