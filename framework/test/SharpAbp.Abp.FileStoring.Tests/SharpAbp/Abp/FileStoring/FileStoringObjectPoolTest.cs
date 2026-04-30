using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;
using SharpAbp.Abp.ObjectPool;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class FileStoringObjectPoolTest
    {
        [Fact]
        public async Task Object_Pool_Policy_Factory_Should_Run_Only_Once_Under_Concurrent_First_Use()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());
            var policyFactoryCallCount = 0;

            var tasks = new Task<IObjectPool<PooledObject>>[32];
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    return orchestrator.GetObjectPool<PooledObject, TestObjectPoolPolicy>(
                        "concurrent-policy",
                        () =>
                        {
                            Interlocked.Increment(ref policyFactoryCallCount);
                            Thread.Sleep(50);
                            return new TestObjectPoolPolicy();
                        });
                });
            }

            await Task.WhenAll(tasks);

            Assert.Equal(1, policyFactoryCallCount);
        }

        [Fact]
        public async Task Async_Object_Pool_Policy_Factory_Should_Run_Only_Once_Under_Concurrent_First_Use()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());
            var policyFactoryCallCount = 0;

            var tasks = new Task<IAsyncObjectPool<PooledObject>>[32];
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    return orchestrator.GetAsyncObjectPool<PooledObject, TestAsyncObjectPoolPolicy>(
                        "concurrent-async-policy",
                        () =>
                        {
                            Interlocked.Increment(ref policyFactoryCallCount);
                            Thread.Sleep(50);
                            return new TestAsyncObjectPoolPolicy();
                        });
                });
            }

            await Task.WhenAll(tasks);

            Assert.Equal(1, policyFactoryCallCount);
        }

        [Fact]
        public async Task Async_Object_Pool_Should_Support_Direct_Policy_Overload()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());
            var policy = new TestAsyncObjectPoolPolicy();

            var pool = orchestrator.GetAsyncObjectPool("direct-async-policy", policy);
            var item = await pool.GetAsync();

            Assert.NotNull(item);
        }

        private class PooledObject
        {
        }

        private class TestObjectPoolPolicy : IObjectPoolPolicy<PooledObject>
        {
            public PooledObject Create()
            {
                return new PooledObject();
            }

            public bool Return(PooledObject obj)
            {
                return obj != null;
            }
        }

        private class TestAsyncObjectPoolPolicy : IAsyncObjectPoolPolicy<PooledObject>
        {
            public ValueTask<PooledObject> CreateAsync()
            {
                return new ValueTask<PooledObject>(new PooledObject());
            }

            public bool Return(PooledObject obj)
            {
                return obj != null;
            }
        }
    }
}
