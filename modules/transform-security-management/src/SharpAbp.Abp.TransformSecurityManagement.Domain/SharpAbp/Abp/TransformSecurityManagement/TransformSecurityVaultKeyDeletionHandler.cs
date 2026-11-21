using System.Linq;
using System.Threading.Tasks;
using SharpAbp.Abp.CryptoVault;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.TransformSecurityManagement;

public sealed class TransformSecurityVaultKeyDeletionHandler(
    ISecurityCredentialInfoRepository repository)
    : ILocalEventHandler<CryptoVaultKeyDeletingEvent>, ITransientDependency
{
    public async Task HandleEventAsync(CryptoVaultKeyDeletingEvent eventData)
    {
        var isReferenced = repository is IReadOnlyRepository<SecurityCredentialInfo> queryable
            ? await queryable.AsyncExecuter.AnyAsync(
                (await queryable.GetQueryableAsync()).Where(credential => credential.CredsId == eventData.KeyId))
            : (await repository.GetListAsync(includeDetails: false)).Any(credential => credential.CredsId == eventData.KeyId);
        if (isReferenced)
        {
            throw new UserFriendlyException("The key is referenced by a transform-security credential.");
        }
    }
}
