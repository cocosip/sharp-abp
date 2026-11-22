using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Guids;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Tokens;
using Volo.Abp.Uow;

namespace SharpAbp.Abp.OpenIddict;

internal sealed class TransactionalApplicationStore(
    IOpenIddictApplicationRepository repository,
    IUnitOfWorkManager unitOfWorkManager,
    IOpenIddictTokenRepository tokenRepository,
    IGuidGenerator guidGenerator,
    AbpOpenIddictIdentifierConverter identifierConverter,
    IOpenIddictDbConcurrencyExceptionHandler concurrencyExceptionHandler,
    IOptions<AbpOpenIddictStoreOptions> storeOptions)
    : AbpOpenIddictApplicationStore(repository, unitOfWorkManager, tokenRepository, guidGenerator,
        identifierConverter, concurrencyExceptionHandler, storeOptions)
{
    public override async ValueTask DeleteAsync(OpenIddictApplicationModel application, CancellationToken cancellationToken)
    {
        if (UnitOfWorkManager.Current == null)
        {
            await base.DeleteAsync(application, cancellationToken);
            return;
        }

        Check.NotNull(application, nameof(application));
        try
        {
            await TokenRepository.DeleteManyByApplicationIdAsync(application.Id, autoSave: false, cancellationToken);
            await Repository.DeleteAsync(application.Id, autoSave: true, cancellationToken);
        }
        catch (AbpDbConcurrencyException exception)
        {
            await ConcurrencyExceptionHandler.HandleAsync(exception);
            throw new OpenIddictExceptions.ConcurrencyException(exception.Message, exception.InnerException);
        }
    }
}
