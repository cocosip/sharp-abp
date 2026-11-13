using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;
using Xunit;

namespace SharpAbp.Abp.ObjectPool
{
    public class ObjectPoolTest
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

        [Fact]
        public void Object_Pool_Should_Allow_Mixing_Direct_Policy_And_Factory_Overload()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());

            var firstPool = orchestrator.GetPool<PooledObject>(
                "mixed-policy-overload",
                new TestPooledObjectPolicy());

            var secondPool = orchestrator.GetPool<PooledObject>(
                "mixed-policy-overload",
                () => new AlternatePooledObjectPolicy());

            Assert.Same(firstPool, secondPool);
        }

        [Fact]
        public void Object_Pool_Should_Use_Default_Maximum_Retained_Consistently()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());

            var firstPool = orchestrator.GetPool<PooledObject>(
                "default-max-size",
                new TestPooledObjectPolicy());

            var secondPool = orchestrator.GetPool<PooledObject>(
                "default-max-size",
                new TestPooledObjectPolicy(),
                ObjectPoolDefaults.GetDefaultMaximumRetained());

            Assert.Same(firstPool, secondPool);
        }

        [Fact]
        public void Bounded_Object_Pool_Should_Dispose_Items_Over_Maximum_Retained()
        {
            var pool = new BoundedObjectPool<DisposablePooledObject>(
                new DisposablePooledObjectPolicy(),
                1);

            var first = pool.Get();
            var second = pool.Get();

            pool.Return(first);
            pool.Return(second);

            Assert.False(first.IsDisposed);
            Assert.True(second.IsDisposed);
        }

        [Fact]
        public void Bounded_Object_Pool_Should_Reuse_Returned_Item()
        {
            var pool = new BoundedObjectPool<DisposablePooledObject>(
                new DisposablePooledObjectPolicy(),
                1);

            var first = pool.Get();
            pool.Return(first);
            var second = pool.Get();

            Assert.Same(first, second);
        }

        [Fact]
        public void Bounded_Object_Pool_Should_Dispose_Retained_Items_On_Dispose()
        {
            var pool = new BoundedObjectPool<DisposablePooledObject>(
                new DisposablePooledObjectPolicy(),
                4);

            var item = pool.Get();
            pool.Return(item);

            pool.Dispose();

            Assert.True(item.IsDisposed);
        }

        [Fact]
        public void Bounded_Object_Pool_Should_Throw_After_Dispose()
        {
            var pool = new BoundedObjectPool<DisposablePooledObject>(
                new DisposablePooledObjectPolicy(),
                4);
            var item = pool.Get();
            pool.Dispose();

            Assert.Throws<ObjectDisposedException>(() => pool.Get());

            pool.Return(item);
            Assert.True(item.IsDisposed);
        }

        [Fact]
        public async Task Async_Object_Pool_Should_Reuse_Returned_Item()
        {
            var pool = new AsyncObjectPool<DisposablePooledObject>(
                new DisposableAsyncObjectPoolPolicy(),
                1);

            var first = await pool.GetAsync();
            pool.Return(first);
            var second = await pool.GetAsync();

            Assert.Same(first, second);
        }

        [Fact]
        public async Task Async_Object_Pool_Should_Dispose_Retained_Items_On_Dispose()
        {
            var pool = new AsyncObjectPool<DisposablePooledObject>(
                new DisposableAsyncObjectPoolPolicy(),
                4);

            var item = await pool.GetAsync();
            pool.Return(item);

            pool.Dispose();

            Assert.True(item.IsDisposed);
        }

        [Fact]
        public async Task Async_Object_Pool_Should_Throw_And_Dispose_Returned_Item_After_Dispose()
        {
            var pool = new AsyncObjectPool<DisposablePooledObject>(
                new DisposableAsyncObjectPoolPolicy(),
                4);
            var item = await pool.GetAsync();
            pool.Dispose();

            Assert.Throws<ObjectDisposedException>(() => pool.GetAsync());

            pool.Return(item);
            Assert.True(item.IsDisposed);
        }

        [Fact]
        public void Object_Pool_RemovePool_Should_Dispose_Retained_Items()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());
            var pool = orchestrator.GetObjectPool<DisposablePooledObject, DisposableObjectPoolPolicy>(
                "remove-pool",
                () => new DisposableObjectPoolPolicy(),
                4);

            var item = pool.Get();
            pool.Return(item);

            Assert.True(orchestrator.RemovePool<DisposablePooledObject>("remove-pool"));
            Assert.True(item.IsDisposed);
            Assert.False(orchestrator.RemovePool<DisposablePooledObject>("remove-pool"));
        }

        [Fact]
        public void Object_Pool_RemovePool_Non_Generic_Should_Dispose_Retained_Items()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());
            var pool = orchestrator.GetObjectPool<DisposablePooledObject, DisposableObjectPoolPolicy>(
                "remove-pool-non-generic",
                () => new DisposableObjectPoolPolicy(),
                4);

            var item = pool.Get();
            pool.Return(item);

            Assert.True(orchestrator.RemovePool(typeof(DisposablePooledObject), "remove-pool-non-generic"));
            Assert.True(item.IsDisposed);
        }

        [Fact]
        public void Object_Pool_RemoveAll_Should_Dispose_All_Retained_Items()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());

            var firstPool = orchestrator.GetObjectPool<DisposablePooledObject, DisposableObjectPoolPolicy>(
                "remove-all-1",
                () => new DisposableObjectPoolPolicy(),
                4);
            var secondPool = orchestrator.GetObjectPool<DisposablePooledObject, DisposableObjectPoolPolicy>(
                "remove-all-2",
                () => new DisposableObjectPoolPolicy(),
                4);

            var first = firstPool.Get();
            var second = secondPool.Get();
            firstPool.Return(first);
            secondPool.Return(second);

            Assert.Equal(2, orchestrator.RemoveAll());
            Assert.True(first.IsDisposed);
            Assert.True(second.IsDisposed);
            Assert.Equal(0, orchestrator.RemoveAll());
        }

        [Fact]
        public void Object_Pool_Dispose_Should_Dispose_All_Retained_Items()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());

            var pool = orchestrator.GetObjectPool<DisposablePooledObject, DisposableObjectPoolPolicy>(
                "dispose-all",
                () => new DisposableObjectPoolPolicy(),
                4);

            var item = pool.Get();
            pool.Return(item);

            orchestrator.Dispose();

            Assert.True(item.IsDisposed);
            Assert.Equal(0, orchestrator.RemoveAll());
        }

        [Fact]
        public void Object_Pool_Get_Should_Create_New_Pool_After_RemovePool()
        {
            var orchestrator = new PoolOrchestrator(new DefaultObjectPoolProvider());

            var firstPool = orchestrator.GetPool<PooledObject>(
                "recreate-pool",
                new TestPooledObjectPolicy());

            Assert.True(orchestrator.RemovePool<PooledObject>("recreate-pool"));

            var secondPool = orchestrator.GetPool<PooledObject>(
                "recreate-pool",
                new TestPooledObjectPolicy());

            Assert.NotSame(firstPool, secondPool);
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

        private class DisposableObjectPoolPolicy : IObjectPoolPolicy<DisposablePooledObject>
        {
            public DisposablePooledObject Create()
            {
                return new DisposablePooledObject();
            }

            public bool Return(DisposablePooledObject obj)
            {
                return obj != null;
            }
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

        private class DisposablePooledObjectPolicy : IPooledObjectPolicy<DisposablePooledObject>
        {
            public DisposablePooledObject Create()
            {
                return new DisposablePooledObject();
            }

            public bool Return(DisposablePooledObject obj)
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
