using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TransformSecurity
{
    public interface ISecurityKeyManager
    {
        Task<SecurityKey> GenerateAsync(string bizType, CancellationToken cancellationToken = default);
    }
}
