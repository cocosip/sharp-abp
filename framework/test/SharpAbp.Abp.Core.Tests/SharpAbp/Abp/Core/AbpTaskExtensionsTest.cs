using SharpAbp.Abp.Core.Extensions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.Core
{
    public class AbpTaskExtensionsTest
    {
        [Fact]
        public void WaitResult_Should_Return_Result_When_Task_Completes_In_Time()
        {
            var task = Task.FromResult(42);

            var result = task.WaitResult(100);

            Assert.Equal(42, result);
        }

        [Fact]
        public void WaitResult_Should_Return_Default_When_Task_Times_Out()
        {
            var taskSource = new TaskCompletionSource<int>();

            var result = taskSource.Task.WaitResult(10);

            Assert.Equal(default, result);
        }

        [Fact]
        public async Task TimeoutAfter_Should_Complete_When_Task_Finishes_In_Time()
        {
            await Task.Delay(10).TimeoutAfter(1000);
        }

        [Fact]
        public async Task TimeoutAfter_Should_Return_Result_When_Generic_Task_Finishes_In_Time()
        {
            var result = await Task.FromResult("ok").TimeoutAfter(1000);

            Assert.Equal("ok", result);
        }

        [Fact]
        public async Task TimeoutAfter_Should_Throw_When_Task_Times_Out()
        {
            var task = Task.Delay(200);

            await Assert.ThrowsAsync<TimeoutException>(() => task.TimeoutAfter(10));
        }

        [Fact]
        public async Task TimeoutAfter_Should_Propagate_Original_Exception()
        {
            var task = Task.FromException<int>(new InvalidOperationException("boom"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => task.TimeoutAfter(1000));

            Assert.Equal("boom", exception.Message);
        }

        [Fact]
        public void WaitResult_Should_Validate_Arguments()
        {
            Task<int> task = null;

            Assert.Throws<ArgumentNullException>(() => task.WaitResult(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Task.FromResult(1).WaitResult(-1));
        }

        [Fact]
        public async Task TimeoutAfter_Should_Validate_Arguments()
        {
            Task task = null;
            Task<int> taskWithResult = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => task.TimeoutAfter(1));
            await Assert.ThrowsAsync<ArgumentNullException>(() => taskWithResult.TimeoutAfter(1));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Task.CompletedTask.TimeoutAfter(-1));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Task.FromResult(1).TimeoutAfter(-1));
        }
    }
}
