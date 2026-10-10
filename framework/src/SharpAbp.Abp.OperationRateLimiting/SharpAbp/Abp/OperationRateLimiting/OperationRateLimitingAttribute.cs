using System;

namespace SharpAbp.Abp.OperationRateLimiting
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class OperationRateLimitingAttribute : Attribute
    {
        public string PolicyName { get; }

        public OperationRateLimitingAttribute(string policyName)
        {
            if (policyName == null)
            {
                throw new ArgumentNullException(nameof(policyName));
            }
            if (string.IsNullOrWhiteSpace(policyName))
            {
                throw new ArgumentException("Policy name must be nonempty.", nameof(policyName));
            }
            PolicyName = policyName;
        }
    }
}
