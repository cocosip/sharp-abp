using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.MinId.EntityFrameworkCore
{
    [ConnectionStringName(MinIdDbProperties.ConnectionStringName)]
    public class MinIdDbContext : AbpDbContext<MinIdDbContext>, IMinIdDbContext
    {
        public DbSet<MinIdInfo> MinIdInfos { get; set; }

        public DbSet<MinIdToken> MinIdTokens { get; set; }

        public MinIdDbContext(DbContextOptions<MinIdDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureMinId();
        }
    }
}