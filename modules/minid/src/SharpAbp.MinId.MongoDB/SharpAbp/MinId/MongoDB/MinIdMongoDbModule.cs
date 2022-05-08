using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace SharpAbp.MinId.MongoDB
{
    [DependsOn(
        typeof(MinIdDomainModule),
        typeof(AbpMongoDbModule)
        )]
    public class MinIdMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<MinIdMongoDbContext>(options =>
            {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, MongoQuestionRepository>();
                 */

            });
        }
    }
}
