#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Pure unit tests for <see cref="DefaultFilePathBuilder"/>.
    /// Uses Moq for ICurrentTenant and IFilePathContextAccessor — no ABP DI container required.
    /// </summary>
    public class DefaultFilePathBuilderTest
    {
        // ─── helpers ────────────────────────────────────────────────────────────

        private static FileProviderSaveArgs MakeArgs(string fileId = "folder/file.jpg") =>
            new FileProviderSaveArgs("test-container", new FileContainerConfiguration(), fileId);

        private static DefaultFilePathBuilder MakeBuilder(
            Guid? tenantId = null,
            string? tenantName = null,
            FilePathContext? context = null,
            Action<AbpFileStoringAbstractionsOptions>? configure = null)
        {
            var currentTenant = new Mock<ICurrentTenant>();
            currentTenant.Setup(t => t.Id).Returns(tenantId);
            currentTenant.Setup(t => t.Name).Returns(tenantName);

            var accessor = new Mock<IFilePathContextAccessor>();
            accessor.Setup(a => a.Current).Returns(context);

            var options = new AbpFileStoringAbstractionsOptions();
            configure?.Invoke(options);

            return new DefaultFilePathBuilder(
                currentTenant.Object,
                accessor.Object,
                Options.Create(options));
        }

        // ─── host paths (no tenant) ──────────────────────────────────────────────

        [Fact]
        public void Build_Host_DefaultPath_Test()
        {
            var builder = MakeBuilder();
            Assert.Equal("host/folder/file.jpg", builder.Build(MakeArgs()));
        }

        [Fact]
        public void Build_Host_EmptyHostSegment_SkipsSegment_Test()
        {
            var builder = MakeBuilder(configure: o => o.FilePathBuilder.HostSegment = "");
            // When HostSegment is empty and there is no prefix, only fileId remains
            Assert.Equal("folder/file.jpg", builder.Build(MakeArgs()));
        }

        [Fact]
        public void Build_Host_CustomHostSegment_Test()
        {
            var builder = MakeBuilder(configure: o => o.FilePathBuilder.HostSegment = "global");
            Assert.Equal("global/folder/file.jpg", builder.Build(MakeArgs()));
        }

        // ─── tenant paths ────────────────────────────────────────────────────────

        [Fact]
        public void Build_Tenant_DefaultGuid_Test()
        {
            var tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var builder = MakeBuilder(tenantId: tenantId);
            Assert.Equal(
                "tenants/3fa85f64-5717-4562-b3fc-2c963f66afa6/folder/file.jpg",
                builder.Build(MakeArgs()));
        }

        [Fact]
        public void Build_Tenant_EmptyTenantsSegment_OnlyIdentifier_Test()
        {
            var tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var builder = MakeBuilder(
                tenantId: tenantId,
                configure: o => o.FilePathBuilder.TenantsSegment = "");
            // No "tenants/" prefix — identifier only
            Assert.Equal(
                "3fa85f64-5717-4562-b3fc-2c963f66afa6/folder/file.jpg",
                builder.Build(MakeArgs()));
        }

        [Fact]
        public void Build_Tenant_CustomTenantsSegment_Test()
        {
            var tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var builder = MakeBuilder(
                tenantId: tenantId,
                configure: o => o.FilePathBuilder.TenantsSegment = "orgs");
            Assert.Equal(
                "orgs/3fa85f64-5717-4562-b3fc-2c963f66afa6/folder/file.jpg",
                builder.Build(MakeArgs()));
        }

        // ─── prefix ─────────────────────────────────────────────────────────────

        [Fact]
        public void Build_StaticPrefix_PrependedBeforeHostSegment_Test()
        {
            var builder = MakeBuilder(configure: o => o.FilePathBuilder.Prefix = "uploads");
            Assert.Equal("uploads/host/folder/file.jpg", builder.Build(MakeArgs()));
        }

        [Fact]
        public void Build_ContextPrefix_OverridesStaticPrefix_Test()
        {
            var ctx = new FilePathContext { Prefix = "prod" };
            var builder = MakeBuilder(context: ctx, configure: o => o.FilePathBuilder.Prefix = "uploads");
            // Context prefix wins over static prefix
            Assert.Equal("prod/host/folder/file.jpg", builder.Build(MakeArgs()));
        }

        [Fact]
        public void Build_PrefixFactory_OverridesContextAndStaticPrefix_Test()
        {
            var ctx = new FilePathContext { Prefix = "ctx-ignored" };
            ctx.Extra["region"] = "cn";
            var builder = MakeBuilder(
                context: ctx,
                configure: o =>
                {
                    o.FilePathBuilder.Prefix = "static-ignored";
                    o.FilePathBuilder.PrefixFactory = c => c?.Extra.GetValueOrDefault("region") as string ?? "default";
                });
            // PrefixFactory has highest priority
            Assert.Equal("cn/host/folder/file.jpg", builder.Build(MakeArgs()));
        }

        [Fact]
        public void Build_PrefixFactory_ReturnsNull_NoPrefix_Test()
        {
            var builder = MakeBuilder(configure: o =>
            {
                o.FilePathBuilder.Prefix = "static-ignored";
                o.FilePathBuilder.PrefixFactory = _ => null;
            });
            Assert.Equal("host/folder/file.jpg", builder.Build(MakeArgs()));
        }

        // ─── TenantIdentifierFactory ─────────────────────────────────────────────

        [Fact]
        public void Build_TenantIdentifierFactory_UsesTenantCode_Test()
        {
            var tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var ctx = new FilePathContext { TenantCode = "H0012" };
            var builder = MakeBuilder(
                tenantId: tenantId,
                context: ctx,
                configure: o => o.FilePathBuilder.TenantIdentifierFactory = (id, name, c) =>
                    c?.TenantCode ?? id.ToString("D"));
            Assert.Equal("tenants/H0012/folder/file.jpg", builder.Build(MakeArgs()));
        }

        [Fact]
        public void Build_TenantIdentifierFactory_FallsBackToGuid_WhenNoCode_Test()
        {
            var tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            // No context → factory falls back to GUID
            var builder = MakeBuilder(
                tenantId: tenantId,
                configure: o => o.FilePathBuilder.TenantIdentifierFactory = (id, name, c) =>
                    c?.TenantCode ?? id.ToString("D"));
            Assert.Equal(
                "tenants/3fa85f64-5717-4562-b3fc-2c963f66afa6/folder/file.jpg",
                builder.Build(MakeArgs()));
        }

        [Fact]
        public void Build_TenantIdentifierFactory_UsesTenantName_Test()
        {
            var tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var builder = MakeBuilder(
                tenantId: tenantId,
                tenantName: "my-org",
                configure: o => o.FilePathBuilder.TenantIdentifierFactory = (id, name, c) =>
                    c?.TenantCode ?? (!string.IsNullOrEmpty(name) ? name! : id.ToString("D")));
            Assert.Equal("tenants/my-org/folder/file.jpg", builder.Build(MakeArgs()));
        }

        // ─── hospital-code scenario (the user's use case) ────────────────────────

        [Fact]
        public void Build_HospitalCode_Prefix_EmptyTenantsSegment_TenantCode_Test()
        {
            // Required path: 0000/医院A/images/photo.jpg
            var tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var ctx = new FilePathContext { TenantCode = "医院A" };
            var builder = MakeBuilder(
                tenantId: tenantId,
                context: ctx,
                configure: o =>
                {
                    o.FilePathBuilder.Prefix = "0000";
                    o.FilePathBuilder.TenantsSegment = "";  // no "tenants/" prefix
                    o.FilePathBuilder.TenantIdentifierFactory = (id, name, c) =>
                        c?.TenantCode ?? id.ToString("D");
                });
            Assert.Equal("0000/医院A/images/photo.jpg", builder.Build(MakeArgs("images/photo.jpg")));
        }

        [Fact]
        public void Build_HospitalCode_NoPrefix_EmptyBothSegments_TenantCode_Test()
        {
            // Required path: 医院A/file.jpg (no prefix, no segment labels)
            var tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var ctx = new FilePathContext { TenantCode = "医院A" };
            var builder = MakeBuilder(
                tenantId: tenantId,
                context: ctx,
                configure: o =>
                {
                    o.FilePathBuilder.TenantsSegment = "";
                    o.FilePathBuilder.HostSegment = "";
                    o.FilePathBuilder.TenantIdentifierFactory = (id, name, c) =>
                        c?.TenantCode ?? id.ToString("D");
                });
            Assert.Equal("医院A/file.jpg", builder.Build(MakeArgs("file.jpg")));
        }

        // ─── DirectFileId strategy ───────────────────────────────────────────────

        [Fact]
        public void Build_DirectFileId_IgnoresAllSegments_Test()
        {
            var tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var builder = MakeBuilder(
                tenantId: tenantId,   // even with a tenant, segments are skipped
                configure: o => o.FilePathStrategy = FilePathGenerationStrategy.DirectFileId);
            Assert.Equal("folder/file.jpg", builder.Build(MakeArgs()));
        }

        [Fact]
        public void Build_DirectFileId_WithStaticPrefix_PrependPrefix_Test()
        {
            var builder = MakeBuilder(configure: o =>
            {
                o.FilePathStrategy = FilePathGenerationStrategy.DirectFileId;
                o.FilePathBuilder.Prefix = "uploads";
            });
            Assert.Equal("uploads/folder/file.jpg", builder.Build(MakeArgs()));
        }

        [Fact]
        public void Build_DirectFileId_WithContextPrefix_ContextPrefixApplied_Test()
        {
            var ctx = new FilePathContext { Prefix = "prod" };
            var builder = MakeBuilder(
                context: ctx,
                configure: o =>
                {
                    o.FilePathStrategy = FilePathGenerationStrategy.DirectFileId;
                    o.FilePathBuilder.Prefix = "uploads"; // overridden by context
                });
            Assert.Equal("prod/folder/file.jpg", builder.Build(MakeArgs()));
        }

        [Fact]
        public void Build_DirectFileId_NoPrefix_ReturnsFileIdOnly_Test()
        {
            var builder = MakeBuilder(configure: o =>
                o.FilePathStrategy = FilePathGenerationStrategy.DirectFileId);
            Assert.Equal("2024/01/report.pdf", builder.Build(MakeArgs("2024/01/report.pdf")));
        }
    }
}
