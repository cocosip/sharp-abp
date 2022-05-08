using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.MinId.MongoDB
{
    [ConnectionStringName(MinIdDbProperties.ConnectionStringName)]
    public class MinIdMongoDbContext : AbpMongoDbContext, IMinIdMongoDbContext
    {
        /* Add mongo collections here. Example:
         * public IMongoCollection<Question> Questions => Collection<Question>();
         */

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);

            modelBuilder.ConfigureMinId();
        }
    }
}