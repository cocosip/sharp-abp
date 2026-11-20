using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.MinId;

public class DefaultSegmentIdServiceTest
{
    private readonly IMinIdInfoRepository _repository = Substitute.For<IMinIdInfoRepository>();
    private readonly IUnitOfWorkManager _manager = Substitute.For<IUnitOfWorkManager>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public DefaultSegmentIdServiceTest()
    {
        _manager.Begin(Arg.Any<AbpUnitOfWorkOptions>(), true).Returns(_unitOfWork);
    }

    [Theory]
    [InlineData(100, 20, 120L)]
    [InlineData(100, 0, 100L)]
    [InlineData(100, 100, 200L)]
    [InlineData(int.MaxValue, 80, 1717987017L)]
    public async Task Allocation_Commits_New_Maximum_And_Computes_Loading_Percentage_Without_Truncation(int step, int percent, long loadingId)
    {
        var info = new MinIdInfo(Guid.NewGuid(), "orders", 100, step, 3, 2);
        _repository.FindByBizTypeAsync("orders", Arg.Any<CancellationToken>()).Returns(info);
        var service = Create(new MinIdOptions { LoadingPercent = percent });

        var segment = await service.GetNextSegmentIdAsync("orders");

        Assert.Equal(100L + step, info.MaxId);
        Assert.Equal(100, segment.CurrentId);
        Assert.Equal(100L + step, segment.MaxId);
        Assert.Equal(loadingId, segment.LoadingId);
        Assert.Equal(3, segment.Delta);
        Assert.Equal(2, segment.Remainder);
        await _unitOfWork.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Missing_Configuration_Rolls_Back_Without_Committing()
    {
        var service = Create(new MinIdOptions());

        var error = await Assert.ThrowsAsync<AbpException>(() => service.GetNextSegmentIdAsync("missing"));

        Assert.Contains("not registered", error.Message);
        await _unitOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Concurrency_Failure_Retries_Then_Reports_Exhausted_Attempts()
    {
        _repository.FindByBizTypeAsync("orders", Arg.Any<CancellationToken>()).ThrowsAsync(new AbpDbConcurrencyException());
        var service = Create(new MinIdOptions { ConflictRetryCount = 2 });

        var error = await Assert.ThrowsAsync<AbpException>(() => service.GetNextSegmentIdAsync("orders"));

        Assert.Contains("after 2 attempts", error.Message);
        await _repository.Received(2).FindByBizTypeAsync("orders", Arg.Any<CancellationToken>());
        await _unitOfWork.Received(2).RollbackAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Rollback_Failure_Preserves_Original_Error()
    {
        _repository.FindByBizTypeAsync("orders", Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException("original"));
        _unitOfWork.RollbackAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException("rollback"));

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Create(new MinIdOptions()).GetNextSegmentIdAsync("orders"));

        Assert.Equal("original", error.Message);
    }

    private DefaultSegmentIdService Create(MinIdOptions options) =>
        new(NullLogger<DefaultSegmentIdService>.Instance, Options.Create(options), _manager, _repository);
}
