using JetBrains.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.FileStoring.Database
{
    public interface IDatabaseFileContainerRepository : IBasicRepository<DatabaseFileContainer, Guid>
    {
        Task<DatabaseFileContainer> FindAsync([NotNull] string name, CancellationToken cancellationToken = default);
    }
}
