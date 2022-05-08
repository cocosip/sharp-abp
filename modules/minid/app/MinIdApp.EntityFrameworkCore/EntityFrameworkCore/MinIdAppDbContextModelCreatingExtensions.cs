using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace MinIdApp.EntityFrameworkCore
{
    public static class MinIdAppDbContextModelCreatingExtensions
    {
        public static void ConfigureMinIdApp(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(MinIdAppConsts.DbTablePrefix + "YourEntities", MinIdAppConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});
        }
    }
}