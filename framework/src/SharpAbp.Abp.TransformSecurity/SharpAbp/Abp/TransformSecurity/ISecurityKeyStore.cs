using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TransformSecurity
{
    public interface ISecurityKeyStore
    {
        Task<SecurityKey> GetAsync(string uniqueId, CancellationToken cancellationToken = default);
        Task SetAsync(SecurityKey securityKey, CancellationToken cancellationToken = default);
    }
}
