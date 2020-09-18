using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class EfCoreFileStoringContainerRepository : EfCoreRepository<IFileStoringManagementDbContext, FileStoringContainer, Guid>,
         IFileStoringContainerRepository
    {
        public EfCoreFileStoringContainerRepository(IDbContextProvider<IFileStoringManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

    }
}
