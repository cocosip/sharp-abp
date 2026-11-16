using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore
{
    public class FileStoringContainerItemModelTest : FileStoringManagementEntityFrameworkCoreTestBase
    {
        [Fact]
        public void Configured_String_Lengths_Fit_Oracle()
        {
            var options = new DbContextOptionsBuilder<FileStoringManagementDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var dbContext = new FileStoringManagementDbContext(options);

            var entityType = dbContext.Model.FindEntityType(typeof(FileStoringContainerItem));
            Assert.NotNull(entityType);

            // A 2048 length exceeds the Oracle NVARCHAR2 character limit and silently becomes an NCLOB.
            var valueMaxLength = entityType.FindProperty(nameof(FileStoringContainerItem.Value)).GetMaxLength();
            Assert.Equal(FileStoringContainerItemConsts.MaxValueLength, valueMaxLength);
            Assert.True(valueMaxLength <= 2000);

            Assert.True(entityType.FindProperty(nameof(FileStoringContainerItem.Name)).GetMaxLength() <= 2000);
        }
    }
}
