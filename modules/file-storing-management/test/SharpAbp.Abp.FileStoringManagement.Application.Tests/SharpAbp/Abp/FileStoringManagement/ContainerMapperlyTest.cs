using System;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace SharpAbp.Abp.FileStoringManagement;

public class ContainerMapperlyTest : FileStoringManagementApplicationTestBase
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Container_Maps_Nested_Items_And_Only_Defined_Extra_Properties(bool existing)
    {
        const string property = "ContainerMapperPublic";
        ObjectExtensionManager.Instance.AddOrUpdateProperty<FileStoringContainer, string>(property);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<ContainerDto, string>(property);
        var source = new FileStoringContainer(Guid.NewGuid(), Guid.NewGuid(), true, "AWS", "files", "Files", true, 100, 20, true);
        source.AddItem(Guid.NewGuid(), "region", "example");
        source.SetProperty(property, "public");
        source.SetProperty("ContainerMapperPrivate", "private");
        var mapper = GetRequiredService<IObjectMapper<FileStoringManagementApplicationModule>>();

        var result = existing ? mapper.Map(source, new ContainerDto()) : mapper.Map<FileStoringContainer, ContainerDto>(source);

        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.TenantId, result.TenantId);
        Assert.True(result.IsMultiTenant);
        Assert.Equal("AWS", result.Provider);
        Assert.Equal("files", result.Name);
        Assert.Equal("Files", result.Title);
        Assert.True(result.EnableAutoMultiPartUpload);
        Assert.Equal(100, result.MultiPartUploadMinFileSize);
        Assert.Equal(20, result.MultiPartUploadShardingSize);
        Assert.True(result.HttpAccess);
        var item = Assert.Single(result.Items);
        Assert.Equal("region", item.Name);
        Assert.Equal("example", item.Value);
        Assert.Equal(source.Id, item.ContainerId);
        Assert.Equal("public", result.ExtraProperties[property]);
        Assert.False(result.ExtraProperties.ContainsKey("ContainerMapperPrivate"));
    }

    [Fact]
    public void Item_Maps_In_Both_Directions_And_Updates_Existing_Destinations()
    {
        var source = new FileStoringContainerItem(Guid.NewGuid(), "region", "example", Guid.NewGuid());
        var mapper = new FileStoringContainerItemToContainerItemDtoMapper();
        var destination = new ContainerItemDto();
        mapper.Map(source, destination);

        foreach (var dto in new[] { mapper.Map(source), destination })
        {
            Assert.Equal(source.Id, dto.Id);
            Assert.Equal(source.ContainerId, dto.ContainerId);
            Assert.Equal(source.Name, dto.Name);
            Assert.Equal(source.Value, dto.Value);
            var reverse = mapper.ReverseMap(dto);
            var existing = new FileStoringContainerItem(Guid.NewGuid(), "old-name", "old-value", Guid.NewGuid());
            var existingId = existing.Id;
            mapper.ReverseMap(dto, existing);
            Assert.Equal(existingId, existing.Id);
            foreach (var result in new[] { reverse, existing })
            {
                Assert.Equal(source.ContainerId, result.ContainerId);
                Assert.Equal(source.Name, result.Name);
                Assert.Equal(source.Value, result.Value);
            }
        }
    }
}
