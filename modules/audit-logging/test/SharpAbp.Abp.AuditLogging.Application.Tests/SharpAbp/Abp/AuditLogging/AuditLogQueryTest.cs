using System;
using System.Threading.Tasks;
using Volo.Abp.AuditLogging;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace SharpAbp.Abp.AuditLogging;

public class AuditLogQueryTest : AuditLoggingApplicationTestBase
{
    [Fact]
    public async Task Get_Returns_Paged_Entry_And_Delete_Removes_It()
    {
        var service = new AuditLogController(GetRequiredService<IAuditLogAppService>());
        var page = await service.GetPagedListAsync(new AuditLogPagedRequestDto());
        var entry = Assert.Single(page.Items);

        var detail = await service.GetAsync(entry.Id);

        Assert.Equal(entry.Id, detail.Id);
        Assert.Equal(entry.ApplicationName, detail.ApplicationName);
        await service.DeleteAsync(entry.Id);
        await Assert.ThrowsAnyAsync<EntityNotFoundException>(() => service.GetAsync(entry.Id));
    }

    [Fact]
    public async Task Entity_Change_Queries_Return_Empty_For_Unknown_Entity()
    {
        var service = new AuditLogController(GetRequiredService<IAuditLogAppService>());

        var page = await service.GetEntityChangePagedListAsync(new EntityChangePagedRequestDto
        {
            AuditLogId = Guid.NewGuid()
        });

        Assert.Equal(0, page.TotalCount);
        Assert.Empty(page.Items);
        Assert.Empty(await service.GetEntityChangesWithUsernameAsync("unknown", "Unknown.Entity"));
    }
}
