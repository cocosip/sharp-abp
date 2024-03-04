using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TransformSecurity
{
    public interface ISecurityCredentialStore
    {
        Task<SecurityCredential> GetAsync(string identifier, CancellationToken cancellationToken = default);
        Task SetAsync(SecurityCredential credential, CancellationToken cancellationToken = default);
    }
}
