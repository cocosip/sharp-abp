using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.FileStoring.EntityFrameworkCore
{
    [ConnectionStringName(AbpFileStoringDbProperties.ConnectionStringName)]
    public interface IFileStoringDbContext : IEfCoreDbContext
    {

    }
}
