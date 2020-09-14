using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring.EntityFrameworkCore
{
    public static class AbpFileStoringDbContextModelCreatingExtensions
    {
        public static void ConfigureFileStoring(
           this ModelBuilder builder,
           [CanBeNull] Action<AbpFileStoringModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new AbpFileStoringModelBuilderConfigurationOptions(
                AbpFileStoringDbProperties.DbTablePrefix,
                AbpFileStoringDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

        }
    }
}
