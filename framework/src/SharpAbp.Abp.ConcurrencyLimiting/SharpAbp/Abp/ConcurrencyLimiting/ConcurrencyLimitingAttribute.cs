using System;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class ConcurrencyLimitingAttribute : Attribute
    {
        public string PolicyName { get; }
        public ConcurrencyLimitingAttribute(string policyName)
        {
            SharpAbpConcurrencyLimitingOptions.ValidateName(policyName, nameof(policyName));
            PolicyName = policyName;
        }
    }

}
