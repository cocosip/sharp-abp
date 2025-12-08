using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring.Minio
{
    public class DefaultMinioFileNameCalculator : IMinioFileNameCalculator, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }
        protected AbpFileStoringAbstractionsOptions Options { get; }

        public DefaultMinioFileNameCalculator(
            ICurrentTenant currentTenant,
            IOptions<AbpFileStoringAbstractionsOptions> options)
        {
            CurrentTenant = currentTenant;
            Options = options.Value;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            if (Options.FilePathStrategy == FilePathGenerationStrategy.DirectFileId)
            {
                return args.FileId;
            }

            return CurrentTenant.Id == null
                ? $"host/{args.FileId}"
                : $"tenants/{CurrentTenant.Id.Value:D}/{args.FileId}";
        }
    }
}
