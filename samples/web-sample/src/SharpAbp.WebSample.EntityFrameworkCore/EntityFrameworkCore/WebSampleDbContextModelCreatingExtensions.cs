using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace SharpAbp.WebSample.EntityFrameworkCore
{
    public static class WebSampleDbContextModelCreatingExtensions
    {
        public static void ConfigureWebSample(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(WebSampleConsts.DbTablePrefix + "YourEntities", WebSampleConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});
        }
    }
}