using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TransformSecurity
{
    public interface ISecurityEncryptionService
    {
        Task<SecurityCredentialValidateResult> ValidateAsync(string identifier, CancellationToken cancellationToken = default);
        Task<string> EncryptAsync(string plainText, string identifier, CancellationToken cancellationToken = default);
        Task<string> DecryptAsync(string cipherText, string identifier, CancellationToken cancellationToken = default);
    }
}
