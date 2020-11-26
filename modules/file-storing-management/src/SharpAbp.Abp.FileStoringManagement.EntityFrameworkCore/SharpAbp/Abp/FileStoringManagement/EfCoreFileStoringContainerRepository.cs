using JetBrains.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;


namespace SharpAbp.Abp.FileStoringManagement
{
    public class EfCoreFileStoringContainerRepository : EfCoreRepository<IFileStoringManagementDbContext, FileStoringContainer, Guid>,
         IFileStoringContainerRepository
    {
        public EfCoreFileStoringContainerRepository(IDbContextProvider<IFileStoringManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// Find container by name
        /// </summary>
        /// <param name="name">container name</param>
        /// <param name="includeDetails">include details</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async virtual Task<FileStoringContainer> FindByNameAsync([NotNull] string name, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return await DbSet
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        }

    }
}
