using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TransformSecurity
{
    public interface ISecurityKeyManager
    {
        Task<SecurityKey> GenerateAsync(CancellationToken cancellationToken = default);
    }
}
