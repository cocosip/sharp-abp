using System;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class DisableConcurrencyLimitingAttribute : Attribute
    {
    }

}
