using System;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class ConcurrencyLimitingParameterAttribute : Attribute
    {
    }

}
