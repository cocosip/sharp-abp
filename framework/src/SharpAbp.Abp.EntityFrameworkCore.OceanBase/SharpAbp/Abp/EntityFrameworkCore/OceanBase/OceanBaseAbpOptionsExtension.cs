using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using OceanBase.EntityFrameworkCore;

namespace SharpAbp.Abp.EntityFrameworkCore.OceanBase
{
    // The provider exposes no public mode accessor; retain the adapter's explicit choice per context.
    internal sealed class OceanBaseAbpOptionsExtension(OceanBaseCompatibilityMode mode) : IDbContextOptionsExtension
    {
        public OceanBaseCompatibilityMode Mode { get; } = mode;
        public DbContextOptionsExtensionInfo Info => new ExtensionInfo(this);
        public void ApplyServices(IServiceCollection services) { }
        public void Validate(IDbContextOptions options) { }

        private sealed class ExtensionInfo(OceanBaseAbpOptionsExtension extension) : DbContextOptionsExtensionInfo(extension)
        {
            public override bool IsDatabaseProvider => false;
            public override string LogFragment => string.Empty;
            public override int GetServiceProviderHashCode() => extension.Mode.GetHashCode();
            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
                => other is ExtensionInfo info && ((OceanBaseAbpOptionsExtension)info.Extension).Mode == extension.Mode;
            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
                => debugInfo["SharpAbp:OceanBaseMode"] = extension.Mode.ToString();
        }
    }
}
