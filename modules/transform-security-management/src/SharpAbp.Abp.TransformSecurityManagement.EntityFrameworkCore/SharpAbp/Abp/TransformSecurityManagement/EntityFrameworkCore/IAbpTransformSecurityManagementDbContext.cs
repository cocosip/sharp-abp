using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.TransformSecurityManagement.EntityFrameworkCore
{
    [ConnectionStringName(AbpTransformSecurityManagementDbProperties.ConnectionStringName)]
    public interface IAbpTransformSecurityManagementDbContext : IEfCoreDbContext
    {
        DbSet<SecurityCredentialInfo> SecurityCredentialInfos { get; set; }
    }
}
