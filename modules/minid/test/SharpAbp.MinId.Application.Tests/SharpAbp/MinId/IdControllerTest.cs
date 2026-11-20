using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.MinId;

public class IdControllerTest : MinIdApplicationTestBase
{
    [Fact]
    public async Task Valid_Token_Generates_Unique_Single_Batch_And_Segment_Results()
    {
        await GetRequiredService<IMinIdTokenAppService>().CreateAsync(new CreateMinIdTokenDto
        {
            BizType = "default", Token = "controllertoken", Remark = "test"
        });
        var controller = new IdController(GetRequiredService<IMinIdTokenValidator>(),
            GetRequiredService<IMinIdGeneratorAppService>(), GetRequiredService<ISegmentIdAppService>());
        // HTTP GET actions run without a transaction; segment allocation owns its own unit of work.
        using var request = GetRequiredService<IUnitOfWorkManager>().Begin(isTransactional: false);

        var first = await controller.SimpleNextIdAsync("default", "Bearer controllertoken");
        var second = await controller.NextIdAsync("default", "Bearer controllertoken");
        var simpleBatch = await controller.SimpleBatchNextIdAsync("default", "Bearer controllertoken", 3);
        var batch = await controller.BatchNextIdAsync("default", "Bearer controllertoken", 3);

        Assert.True(second.Success);
        Assert.NotEqual(first, second.Data);
        Assert.Equal(3, simpleBatch.Count);
        Assert.True(batch.Success);
        Assert.Equal(3, batch.Data.Count);
        Assert.Equal(8, simpleBatch.Concat(batch.Data).Append(first).Append(second.Data).Distinct().Count());
        var segment = await controller.GetNextSegmentIdAsync("default", "Bearer controllertoken");
        Assert.True(segment.MaxId >= segment.CurrentId);
        Assert.Equal(1, segment.Delta);
    }

    [Fact]
    public async Task Invalid_Token_Returns_Failure_Envelope_Or_Throws_For_Simple_Endpoints()
    {
        var controller = new IdController(GetRequiredService<IMinIdTokenValidator>(),
            GetRequiredService<IMinIdGeneratorAppService>(), GetRequiredService<ISegmentIdAppService>());

        var single = await controller.NextIdAsync("default", "Bearer missing");
        var batch = await controller.BatchNextIdAsync("default", "Bearer missing", 3);

        Assert.False(single.Success);
        Assert.Equal(0, single.Data);
        Assert.Contains("Invalid token", single.Message);
        Assert.False(batch.Success);
        Assert.Null(batch.Data);
        Assert.Contains("Invalid token", batch.Message);
        await Assert.ThrowsAnyAsync<AbpException>(() => controller.SimpleNextIdAsync("default", "Bearer missing"));
        await Assert.ThrowsAnyAsync<AbpException>(() => controller.SimpleBatchNextIdAsync("default", "Bearer missing", 3));
        await Assert.ThrowsAnyAsync<AbpException>(() => controller.GetNextSegmentIdAsync("default", "Bearer missing"));
    }
}
