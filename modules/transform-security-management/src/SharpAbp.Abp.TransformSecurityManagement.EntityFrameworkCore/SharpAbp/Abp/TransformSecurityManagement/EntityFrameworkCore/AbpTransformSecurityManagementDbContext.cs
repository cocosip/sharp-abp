using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.TransformSecurityManagement.EntityFrameworkCore
{
    [ConnectionStringName(AbpTransformSecurityManagementDbProperties.ConnectionStringName)]
    public class AbpTransformSecurityManagementDbContext : AbpDbContext<AbpTransformSecurityManagementDbContext>, IAbpTransformSecurityManagementDbContext
    {
        public DbSet<SecurityCredentialInfo> SecurityCredentialInfos { get; set; }

        public AbpTransformSecurityManagementDbContext(DbContextOptions<AbpTransformSecurityManagementDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigureTransformSecurityManagement();
        }

    }
}
