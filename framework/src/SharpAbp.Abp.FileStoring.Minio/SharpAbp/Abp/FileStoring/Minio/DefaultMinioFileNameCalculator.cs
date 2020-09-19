using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring.Minio
{
    public class DefaultMinioFileNameCalculator : IMinioFileNameCalculator, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }

        public DefaultMinioFileNameCalculator(ICurrentTenant currentTenant)
        {
            CurrentTenant = currentTenant;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            return CurrentTenant.Id == null
                ? $"host/{args.FileId}"
                : $"tenants/{CurrentTenant.Id.Value:D}/{args.FileId}";
        }
    }
}
