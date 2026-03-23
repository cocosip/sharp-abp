using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Unit tests for <see cref="AsyncLocalFilePathContextAccessor"/>.
    /// Verifies the ambient-context behaviour: Change/Dispose, nesting, and async propagation.
    /// </summary>
    public class AsyncLocalFilePathContextAccessorTest
    {
        // Each test gets its own isolated AsyncLocal scope because xUnit
        // runs tests in separate execution contexts (threads / async tasks).
        // We still call Change() and dispose to be explicit and safe.

        private readonly IFilePathContextAccessor _accessor = AsyncLocalFilePathContextAccessor.Instance;

        [Fact]
        public void Initial_Current_IsNull_Test()
        {
            // Before any Change() call the current context must be null
            Assert.Null(_accessor.Current);
        }

        [Fact]
        public void Change_SetsCurrentContext_Test()
        {
            var ctx = new FilePathContext { TenantCode = "T001" };
            using (_accessor.Change(ctx))
            {
                Assert.Same(ctx, _accessor.Current);
                Assert.Equal("T001", _accessor.Current!.TenantCode);
            }
        }

        [Fact]
        public void Change_Dispose_RestoresNullContext_Test()
        {
            var ctx = new FilePathContext { TenantCode = "T001" };
            var scope = _accessor.Change(ctx);
            Assert.NotNull(_accessor.Current);

            scope.Dispose();
            Assert.Null(_accessor.Current);
        }

        [Fact]
        public void Change_Dispose_RestoresPreviousContext_Test()
        {
            var outer = new FilePathContext { TenantCode = "outer" };
            var inner = new FilePathContext { TenantCode = "inner" };

            using (_accessor.Change(outer))
            {
                Assert.Equal("outer", _accessor.Current!.TenantCode);

                using (_accessor.Change(inner))
                {
                    Assert.Equal("inner", _accessor.Current!.TenantCode);
                }

                // After inner scope is disposed, outer context is restored
                Assert.Equal("outer", _accessor.Current!.TenantCode);
            }

            Assert.Null(_accessor.Current);
        }

        [Fact]
        public void TripleNested_Change_RestoredInOrder_Test()
        {
            var ctx1 = new FilePathContext { TenantCode = "L1" };
            var ctx2 = new FilePathContext { TenantCode = "L2" };
            var ctx3 = new FilePathContext { TenantCode = "L3" };

            using (_accessor.Change(ctx1))
            {
                Assert.Equal("L1", _accessor.Current!.TenantCode);
                using (_accessor.Change(ctx2))
                {
                    Assert.Equal("L2", _accessor.Current!.TenantCode);
                    using (_accessor.Change(ctx3))
                    {
                        Assert.Equal("L3", _accessor.Current!.TenantCode);
                    }
                    Assert.Equal("L2", _accessor.Current!.TenantCode);
                }
                Assert.Equal("L1", _accessor.Current!.TenantCode);
            }
            Assert.Null(_accessor.Current);
        }

        [Fact]
        public void Change_OverrideWithNull_ClearsContext_Test()
        {
            var ctx = new FilePathContext { TenantCode = "T001" };
            using (_accessor.Change(ctx))
            {
                // Override with null inside the outer scope
                using (_accessor.Change(null))
                {
                    Assert.Null(_accessor.Current);
                }
                // Restored to outer context after null scope ends
                Assert.Equal("T001", _accessor.Current!.TenantCode);
            }
        }

        [Fact]
        public void Change_ContextPrefix_IsAccessible_Test()
        {
            var ctx = new FilePathContext { Prefix = "uploads", TenantCode = "T001" };
            using (_accessor.Change(ctx))
            {
                Assert.Equal("uploads", _accessor.Current!.Prefix);
                Assert.Equal("T001", _accessor.Current!.TenantCode);
            }
        }

        [Fact]
        public void Change_ExtraProperties_ArePreserved_Test()
        {
            var ctx = new FilePathContext();
            ctx.Extra["region"] = "cn-east";
            ctx.Extra["env"] = "prod";

            using (_accessor.Change(ctx))
            {
                Assert.Equal("cn-east", _accessor.Current!.Extra["region"]);
                Assert.Equal("prod", _accessor.Current!.Extra["env"]);
            }
        }

        [Fact]
        public async Task Change_ContextFlowsAcrossAwait_Test()
        {
            var ctx = new FilePathContext { TenantCode = "async-tenant" };
            using (_accessor.Change(ctx))
            {
                // Simulate async work — AsyncLocal propagates into child tasks
                await Task.Yield();
                Assert.Equal("async-tenant", _accessor.Current!.TenantCode);
            }
        }

        [Fact]
        public void Instance_IsSingletonReference_Test()
        {
            var instance1 = AsyncLocalFilePathContextAccessor.Instance;
            var instance2 = AsyncLocalFilePathContextAccessor.Instance;
            Assert.Same(instance1, instance2);
        }
    }
}
