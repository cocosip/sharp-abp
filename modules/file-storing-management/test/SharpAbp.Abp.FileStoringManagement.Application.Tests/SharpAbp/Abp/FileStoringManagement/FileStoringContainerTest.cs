using System;
using Xunit;

namespace SharpAbp.Abp.FileStoringManagement;

public class FileStoringContainerTest
{
    [Fact]
    public void Update_Preserves_Tenant_And_Reports_Name_Change()
    {
        var tenantId = Guid.NewGuid();
        var container = new FileStoringContainer(Guid.NewGuid(), tenantId, false, "FileSystem", "original", "Original", false, 1, 2, false);

        container.Update(true, "AWS", "renamed", "Updated", true, 100, 20, true);

        Assert.Equal(tenantId, container.TenantId);
        Assert.True(container.IsMultiTenant);
        Assert.Equal("AWS", container.Provider);
        Assert.Equal("renamed", container.Name);
        Assert.Equal("Updated", container.Title);
        Assert.True(container.EnableAutoMultiPartUpload);
        Assert.Equal(100, container.MultiPartUploadMinFileSize);
        Assert.Equal(20, container.MultiPartUploadShardingSize);
        Assert.True(container.HttpAccess);
        var change = Assert.IsType<FileStoringContainerUpdatedEto>(Assert.Single(container.GetDistributedEvents()).EventData);
        Assert.Equal(container.Id, change.Id);
        Assert.Equal(tenantId, change.TenantId);
        Assert.Equal("original", change.OldName);
        Assert.Equal("renamed", change.Name);
    }

    [Fact]
    public void Configuration_Items_Belong_To_Container_And_Can_Be_Removed()
    {
        var container = new FileStoringContainer(Guid.NewGuid(), null, false, "AWS", "name", "title", false, 0, 0, false);
        var itemId = Guid.NewGuid();
        container.AddItem(itemId, "region", "example");
        container.AddItem(Guid.NewGuid(), "bucket", "test");

        var region = Assert.Single(container.Items, x => x.Name == "region");
        Assert.Equal(itemId, region.Id);
        Assert.Equal(container.Id, region.ContainerId);
        Assert.Equal("example", region.Value);
        container.RemoveItem("missing");
        Assert.Equal(2, container.Items.Count);
        container.RemoveItem("region");
        Assert.Equal("bucket", Assert.Single(container.Items).Name);
        container.ItemClear();
        Assert.Empty(container.Items);
    }
}
