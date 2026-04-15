using SharpAbp.Abp.Core.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.Core
{
    public class AbpTaskFactoryExtensionsTest
    {
        [Fact]
        public async Task StartDelayedTask_Should_Run_Action()
        {
            var executed = false;
            var factory = new TaskFactory();

            var task = factory.StartDelayedTask(10, () => executed = true);

            await task.TimeoutAfter(1000);

            Assert.True(executed);
        }

        [Fact]
        public async Task StartDelayedTask_Should_Return_Canceled_Task_When_Token_Is_Already_Canceled()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var factory = new TaskFactory(cancellationTokenSource.Token);

            var task = factory.StartDelayedTask(10, () => { });

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);
            Assert.True(task.IsCanceled);
        }

        [Fact]
        public async Task StartDelayedTask_Should_Validate_Arguments()
        {
            TaskFactory factory = null;
            Action action = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => factory.StartDelayedTask(1, () => { }));
            await Assert.ThrowsAsync<ArgumentNullException>(() => new TaskFactory().StartDelayedTask(1, action));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => new TaskFactory().StartDelayedTask(-1, () => { }));
        }
    }
}
