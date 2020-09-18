using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class EfCoreFileStoringContainerItemRepository : EfCoreRepository<IFileStoringManagementDbContext, FileStoringContainerItem, Guid>,
         IFileStoringContainerItemRepository
    {
        public EfCoreFileStoringContainerItemRepository(IDbContextProvider<IFileStoringManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }
    }
}
