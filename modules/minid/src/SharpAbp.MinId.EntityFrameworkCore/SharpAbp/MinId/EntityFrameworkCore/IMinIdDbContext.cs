using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.MinId.EntityFrameworkCore
{
    [ConnectionStringName(MinIdDbProperties.ConnectionStringName)]
    public interface IMinIdDbContext : IEfCoreDbContext
    {
        public DbSet<MinIdInfo> MinIdInfos { get; set; }

        public DbSet<MinIdToken> MinIdTokens { get; set; }
    }
}