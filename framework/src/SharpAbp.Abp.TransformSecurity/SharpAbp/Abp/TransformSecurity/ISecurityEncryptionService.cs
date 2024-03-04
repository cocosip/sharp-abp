using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TransformSecurity
{
    public interface ISecurityEncryptionService
    {
        Task<SecurityCredential> GenerateAsync(CancellationToken cancellationToken = default);
        Task<string> DecryptAsync(string cipherText, string id, CancellationToken cancellationToken = default);
    }
}
