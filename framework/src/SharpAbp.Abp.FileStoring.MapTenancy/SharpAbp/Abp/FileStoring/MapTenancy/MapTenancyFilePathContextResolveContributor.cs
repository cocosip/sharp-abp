using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.MapTenancy;
using Volo.Abp;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring.MapTenancy
{
    public class MapTenancyFilePathContextResolveContributor : IFilePathContextResolveContributor
    {
        public const string ContributorName = "MapTenancy";

        public string Name => ContributorName;

        public virtual async Task ResolveAsync(IFilePathContextResolveContext context)
        {
            var currentTenant = context.ServiceProvider.GetRequiredService<ICurrentTenant>();
            if (!currentTenant.Id.HasValue)
            {
                return;
            }

            var options = context
                .ServiceProvider
                .GetRequiredService<IOptions<AbpFileStoringMapTenancyOptions>>()
                .Value;

            var codeProvider = context.ServiceProvider.GetRequiredService<IMapTenantCodeProvider>();
            var codeInfo = await codeProvider.FindByTenantIdAsync(currentTenant.Id.Value);
            if (codeInfo == null)
            {
                if (options.MissingMapTenantBehavior == MissingMapTenantBehavior.Throw)
                {
                    throw new AbpException($"Could not find map tenant code by tenant id '{currentTenant.Id.Value}'.");
                }

                return;
            }

            var tenantCode = options.TenantCodeSource == FilePathTenantCodeSource.MapCode
                ? codeInfo.MapCode
                : codeInfo.Code;
            if (string.IsNullOrWhiteSpace(tenantCode))
            {
                return;
            }

            context.FilePathContext = new FilePathContext
            {
                TenantCode = tenantCode
            };
            context.FilePathContext.Extra["TenantId"] = codeInfo.TenantId;
            context.FilePathContext.Extra["TenantName"] = codeInfo.TenantName;
            context.FilePathContext.Extra["Code"] = codeInfo.Code;
            context.FilePathContext.Extra["MapCode"] = codeInfo.MapCode;
            context.FilePathContext.Extra["Source"] = ContributorName;
            context.Handled = true;
        }
    }
}
