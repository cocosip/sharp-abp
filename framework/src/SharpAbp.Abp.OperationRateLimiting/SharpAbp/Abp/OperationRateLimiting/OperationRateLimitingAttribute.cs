using System;

namespace SharpAbp.Abp.OperationRateLimiting
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class OperationRateLimitingAttribute : Attribute
    {
        public string PolicyName { get; }

        public OperationRateLimitingAttribute(string policyName)
        {
            PolicyName = policyName;
        }
    }
}
