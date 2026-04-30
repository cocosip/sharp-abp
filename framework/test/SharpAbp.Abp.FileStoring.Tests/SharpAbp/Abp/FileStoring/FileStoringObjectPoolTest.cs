using System;
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

        [Fact]
        public void Object_Pool_Non_Generic_Policy_Factory_Should_Run_Only_On_First_Creation()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());
            var policyFactoryCallCount = 0;

            var firstPool = orchestrator.GetPool<PooledObject>(
                "non-generic-factory-first-use",
                () =>
                {
                    policyFactoryCallCount++;
                    return new TestPooledObjectPolicy();
                });

            var secondPool = orchestrator.GetPool<PooledObject>(
                "non-generic-factory-first-use",
                () =>
                {
                    policyFactoryCallCount++;
                    return new AlternatePooledObjectPolicy();
                });

            Assert.Same(firstPool, secondPool);
            Assert.Equal(1, policyFactoryCallCount);
        }

        [Fact]
        public void Object_Pool_Abstraction_Non_Generic_Policy_Factory_Should_Run_Only_On_First_Creation()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());
            var policyFactoryCallCount = 0;

            var firstPool = orchestrator.GetObjectPool<PooledObject>(
                "object-pool-non-generic-factory-first-use",
                () =>
                {
                    policyFactoryCallCount++;
                    return new TestObjectPoolPolicy();
                });

            var secondPool = orchestrator.GetObjectPool<PooledObject>(
                "object-pool-non-generic-factory-first-use",
                () =>
                {
                    policyFactoryCallCount++;
                    return new AlternateObjectPoolPolicy();
                });

            Assert.NotSame(firstPool, secondPool);
            Assert.Equal(1, policyFactoryCallCount);
        }

        [Fact]
        public async Task Async_Object_Pool_Non_Generic_Policy_Factory_Should_Run_Only_On_First_Creation()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());
            var policyFactoryCallCount = 0;

            var firstPool = orchestrator.GetAsyncObjectPool<PooledObject>(
                "async-pool-non-generic-factory-first-use",
                () =>
                {
                    policyFactoryCallCount++;
                    return new TestAsyncObjectPoolPolicy();
                });

            var secondPool = orchestrator.GetAsyncObjectPool<PooledObject>(
                "async-pool-non-generic-factory-first-use",
                () =>
                {
                    policyFactoryCallCount++;
                    return new AlternateAsyncObjectPoolPolicy();
                });

            Assert.Same(firstPool, secondPool);
            Assert.NotNull(await secondPool.GetAsync());
            Assert.Equal(1, policyFactoryCallCount);
        }

        [Fact]
        public async Task Async_Object_Pool_Should_Dispose_Rejected_Items()
        {
            var pool = new AsyncObjectPool<DisposablePooledObject>(
                new RejectingDisposableAsyncObjectPoolPolicy(),
                1);
            var item = await pool.GetAsync();

            pool.Return(item);

            Assert.True(item.IsDisposed);
        }

        [Fact]
        public async Task Async_Object_Pool_Should_Dispose_Items_Over_Maximum_Retained()
        {
            var pool = new AsyncObjectPool<DisposablePooledObject>(
                new DisposableAsyncObjectPoolPolicy(),
                0);
            var item = await pool.GetAsync();

            pool.Return(item);

            Assert.True(item.IsDisposed);
        }

        [Fact]
        public void Object_Pool_Should_Reject_Different_Concrete_Policy_For_Existing_Pool()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());

            orchestrator.GetPool<PooledObject>(
                "same-name-different-policy",
                new TestPooledObjectPolicy());

            Assert.Throws<InvalidOperationException>(() =>
            {
                orchestrator.GetPool<PooledObject>(
                    "same-name-different-policy",
                    new AlternatePooledObjectPolicy());
            });
        }

        [Fact]
        public void Object_Pool_Factory_Should_Retry_After_First_Creation_Failure()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());
            var attempts = 0;

            Assert.Throws<InvalidOperationException>(() =>
            {
                orchestrator.GetPool<PooledObject>(
                    "retry-after-failure",
                    () =>
                    {
                        attempts++;
                        throw new InvalidOperationException("Factory failed.");
                    });
            });

            var pool = orchestrator.GetPool<PooledObject>(
                "retry-after-failure",
                () =>
                {
                    attempts++;
                    return new TestPooledObjectPolicy();
                });

            Assert.NotNull(pool.Get());
            Assert.Equal(2, attempts);
        }

        private class PooledObject
        {
        }

        private class DisposablePooledObject : IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
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

        private class AlternateObjectPoolPolicy : TestObjectPoolPolicy
        {
        }

        private class TestPooledObjectPolicy : IPooledObjectPolicy<PooledObject>
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

        private class AlternatePooledObjectPolicy : TestPooledObjectPolicy
        {
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

        private class AlternateAsyncObjectPoolPolicy : TestAsyncObjectPoolPolicy
        {
        }

        private class DisposableAsyncObjectPoolPolicy : IAsyncObjectPoolPolicy<DisposablePooledObject>
        {
            public ValueTask<DisposablePooledObject> CreateAsync()
            {
                return new ValueTask<DisposablePooledObject>(new DisposablePooledObject());
            }

            public virtual bool Return(DisposablePooledObject obj)
            {
                return obj != null;
            }
        }

        private class RejectingDisposableAsyncObjectPoolPolicy : DisposableAsyncObjectPoolPolicy
        {
            public override bool Return(DisposablePooledObject obj)
            {
                return false;
            }
        }
    }
}
