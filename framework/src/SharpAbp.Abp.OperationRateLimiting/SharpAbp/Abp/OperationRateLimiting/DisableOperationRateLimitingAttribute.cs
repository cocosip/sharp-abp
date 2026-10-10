using System;

namespace SharpAbp.Abp.OperationRateLimiting
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class DisableOperationRateLimitingAttribute : Attribute
    {
    }
}
