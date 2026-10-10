using System.IO;
using System.Security.Cryptography;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingKeyBuilder : IOperationRateLimitingKeyBuilder, ISingletonDependency
    {
        public virtual string Build(string applicationName, string policyName,
            OperationRateLimitingRuleDefinition rule, string partition, string tenant)
        {
            return "operation-rate:v1:" + HashComponents(applicationName, "v1", policyName, rule.Name, partition,
                rule.MultiTenancy ? tenant : "global-tenancy");
        }

        public static string HashComponents(params string[] components)
        {
            using (var stream = new MemoryStream())
            {
                foreach (var component in components)
                {
                    var bytes = Encoding.UTF8.GetBytes(component);
                    var length = bytes.Length;
                    stream.WriteByte((byte)(length >> 24));
                    stream.WriteByte((byte)(length >> 16));
                    stream.WriteByte((byte)(length >> 8));
                    stream.WriteByte((byte)length);
                    stream.Write(bytes, 0, bytes.Length);
                }

                using (var sha = SHA256.Create())
                {
                    var digest = sha.ComputeHash(stream.ToArray());
                    var result = new StringBuilder(digest.Length * 2);
                    foreach (var value in digest)
                    {
                        result.Append(value.ToString("x2", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    return result.ToString();
                }
            }
        }
    }
}
