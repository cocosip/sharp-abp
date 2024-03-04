using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TransformSecurity
{
    public interface ISecurityCredentialManager
    {
        Task<SecurityCredential> GenerateAsync(string bizType, CancellationToken cancellationToken = default);
    }
}
