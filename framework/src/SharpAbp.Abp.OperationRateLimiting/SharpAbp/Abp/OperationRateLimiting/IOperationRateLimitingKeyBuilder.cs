using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public interface IOperationRateLimitingKeyBuilder
    {
        string Build(string applicationName, string policyName, OperationRateLimitingRuleDefinition rule,
            string partition, string tenant);
    }
}
