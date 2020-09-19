using JetBrains.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.FileStoring.Database
{

    public interface IDatabaseFileRepository : IBasicRepository<DatabaseFile, Guid>
    {
        Task<DatabaseFile> FindAsync(Guid containerId, [NotNull] string name, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid containerId, [NotNull] string name, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid containerId, [NotNull] string name, bool autoSave = false, CancellationToken cancellationToken = default);
    }
}
