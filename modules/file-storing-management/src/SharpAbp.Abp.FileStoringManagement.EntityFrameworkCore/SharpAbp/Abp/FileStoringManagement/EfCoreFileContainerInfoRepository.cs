using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class EfCoreFileContainerInfoRepository : EfCoreRepository<IFileStoringManagementDbContext, FileContainerInfo, Guid>,
         IFileContainerInfoRepository
    {
        public EfCoreFileContainerInfoRepository(IDbContextProvider<IFileStoringManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

    }
}
